using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.LessonDtos;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.Implementations
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public LessonService(
            ILessonRepository lessonRepository,
            ICourseRepository courseRepository,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _lessonRepository = lessonRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<LessonDto> CreateAsync(LessonDto lessonDto, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Teacher"))
                throw new Exception("Only teachers can create lessons.");

            var course = await _courseRepository.GetByIdAsync(lessonDto.CourseId);
            if (course == null)
                throw new Exception("Course not found.");

            var lesson = _mapper.Map<Lesson>(lessonDto);
            lesson.Id = Guid.NewGuid();
            lesson.CourseId = course.Id;

            await _lessonRepository.AddAsync(lesson);
            return _mapper.Map<LessonDto>(lesson);
        }

        public async Task UpdateAsync(LessonDto lessonDto, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Teacher"))
                throw new Exception("Only teachers can update lessons.");

            if (lessonDto.Id == Guid.Empty)
                throw new Exception("Lesson ID is required.");

            var lesson = await _lessonRepository.GetByIdAsync(lessonDto.Id);
            if (lesson == null)
                throw new Exception("Lesson not found.");

            var course = await _courseRepository.GetByIdAsync(lessonDto.CourseId);
            if (course == null)
                throw new Exception("Course not found.");

            _mapper.Map(lessonDto, lesson);
            lesson.CourseId = course.Id;

            await _lessonRepository.UpdateAsync(lesson);
        }

        public async Task DeleteAsync(Guid id, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Teacher"))
                throw new Exception("Only teachers can delete lessons.");

            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null)
                throw new Exception("Lesson not found.");

            await _lessonRepository.DeleteAsync(id);
        }
    }
}
