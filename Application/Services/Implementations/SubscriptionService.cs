﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.SubscriptionDtos;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.Implementations
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepository,
            ILessonRepository lessonRepository,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _subscriptionRepository = subscriptionRepository;
            _lessonRepository = lessonRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<SubscriptionDto> CreateSubscriptionAsync(SubscriptionDto subscriptionDto, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            var subscriptionType = Enum.Parse<SubscriptionType>(subscriptionDto.Type);

            var subscription = _mapper.Map<Subscription>(subscriptionDto);
            subscription.Id = Guid.NewGuid();
            subscription.UserId = userId;

            if (subscription.Grade != user.Grade)
                throw new Exception("Subscription grade must match user's grade.");

            if (subscriptionType == SubscriptionType.Yearly)
            {
                var currentYear = DateTime.UtcNow.Year;
                subscription.StartDate = new DateTime(currentYear, 1, 8, 0, 0, 0, DateTimeKind.Utc);
                subscription.EndDate = new DateTime(currentYear + 1, 1, 8, 0, 0, 0, DateTimeKind.Utc);
                subscription.SubscribedMonths = Enumerable.Range(1, 12).ToList(); // Full year access
            }
            else if (subscriptionType == SubscriptionType.LectureBased)
            {
                if (subscription.LectureCount == null || subscription.LectureCount <= 0)
                    throw new Exception("Lecture count must be specified and greater than zero for lecture-based subscriptions.");
                if (subscription.Price <= 0)
                    throw new Exception("Price must be greater than zero for lecture-based subscriptions.");
            }
            else
            {
                subscription.LectureCount = null;
                subscription.Price = 0; // Default for non-lecture-based subscriptions
            }

            await _subscriptionRepository.AddAsync(subscription);
            return _mapper.Map<SubscriptionDto>(subscription);
        }

        public async Task<SubscriptionDto> CreateLectureBasedSubscriptionAsync(SubscriptionDto subscriptionDto, string teacherId)
        {
            var teacher = await _userManager.FindByIdAsync(teacherId);
            if (teacher == null || !await _userManager.IsInRoleAsync(teacher, "Teacher"))
                throw new Exception("Only teachers can create lecture-based subscriptions.");

            var subscriptionType = Enum.Parse<SubscriptionType>(subscriptionDto.Type);
            if (subscriptionType != SubscriptionType.LectureBased)
                throw new Exception("Only lecture-based subscriptions can be created via this endpoint.");

            if (subscriptionDto.LectureCount == null || subscriptionDto.LectureCount <= 0)
                throw new Exception("Lecture count must be specified and greater than zero.");
            if (subscriptionDto.Price <= 0)
                throw new Exception("Price must be greater than zero.");

            var subscription = _mapper.Map<Subscription>(subscriptionDto);
            subscription.Id = Guid.NewGuid();

            await _subscriptionRepository.AddAsync(subscription);
            return _mapper.Map<SubscriptionDto>(subscription);
        }

        public async Task<bool> CanAccessLessonAsync(string userId, Guid lessonId)
        {
            var lesson = await _lessonRepository.GetByIdAsync(lessonId);
            if (lesson == null)
                return false;

            // Check if user has redeemed a code for this lesson
            var hasAccessCode = await _lessonRepository.HasAccessCodeAsync(lessonId, userId);
            if (hasAccessCode)
                return true;

            var subscriptions = await _subscriptionRepository.GetByUserIdAsync(userId);

            foreach (var subscription in subscriptions)
            {
                if (subscription.Grade != lesson.Course.EducationalLevel)
                    continue;

                if (subscription.Type == SubscriptionType.Monthly ||
                    subscription.Type == SubscriptionType.Yearly)
                {
                    if (subscription.SubscribedMonths.Contains(DateTime.UtcNow.Month))
                        return true;
                }
                else if (subscription.Type == SubscriptionType.LectureBased)
                {
                    if (subscription.LectureCount > subscription.AccessedLessons.Count &&
                        !subscription.AccessedLessons.Contains(lessonId))
                        return true;
                }
            }

            return lesson.IsFree;
        }

        public async Task MarkLessonAsAccessedAsync(string userId, Guid lessonId)
        {
            var lesson = await _lessonRepository.GetByIdAsync(lessonId);
            if (lesson == null)
                throw new Exception("Lesson not found.");

            // Check if access was granted via a code
            var hasAccessCode = await _lessonRepository.HasAccessCodeAsync(lessonId, userId);
            if (hasAccessCode)
                return; // Access already granted via code and no need to mark in subscription

            var subscriptions = await _subscriptionRepository.GetByUserIdAsync(userId);
            var validSubscription = subscriptions.FirstOrDefault(s =>
                s.Type == SubscriptionType.LectureBased &&
                s.Grade == lesson.Course.EducationalLevel &&
                s.LectureCount > s.AccessedLessons.Count &&
                !s.AccessedLessons.Contains(lessonId));

            if (validSubscription == null)
                throw new Exception("No valid subscription to access this lesson.");

            validSubscription.AccessedLessons.Add(lessonId);
            await _subscriptionRepository.UpdateAsync(validSubscription);
        }

        public async Task DeleteUserSubscriptionsAsync(string userId, string teacherId)
        {
            var teacher = await _userManager.FindByIdAsync(teacherId);
            if (teacher == null || !await _userManager.IsInRoleAsync(teacher, "Teacher"))
                throw new Exception("Only teachers can delete subscriptions.");

            await _subscriptionRepository.DeleteByUserIdAsync(userId);
        }

        public async Task DeleteAllSubscriptionsAsync(string teacherId)
        {
            var teacher = await _userManager.FindByIdAsync(teacherId);
            if (teacher == null || !await _userManager.IsInRoleAsync(teacher, "Teacher"))
                throw new Exception("Only teachers can delete subscriptions.");

            var subscriptions = await _subscriptionRepository.GetAllAsync();
            foreach (var subscription in subscriptions)
            {
                await _subscriptionRepository.DeleteAsync(subscription.Id);
            }

        }
    }
}
