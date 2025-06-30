using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.CourseDtos;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public CourseService(ICourseRepository courseRepository, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<CourseDto> GetByIdAsync(Guid id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                throw new Exception("Course not found.");
            return _mapper.Map<CourseDto>(course);
        }

        public async Task<List<CourseDto>> GetAllAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            return _mapper.Map<List<CourseDto>>(courses);
        }

        public async Task<CourseDto> CreateAsync(CourseDto courseDto, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Teacher"))
                throw new Exception("Only teachers can create courses.");

            var course = _mapper.Map<Course>(courseDto);
            course.Id = Guid.NewGuid();
            await _courseRepository.AddAsync(course);
            return _mapper.Map<CourseDto>(course);
        }

        public async Task UpdateAsync(CourseDto courseDto, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Teacher"))
                throw new Exception("Only teachers can update courses.");

            var course = await _courseRepository.GetByIdAsync(courseDto.Id);
            if (course == null)
                throw new Exception("Course not found.");

            _mapper.Map(courseDto, course);
            await _courseRepository.UpdateAsync(course);
        }

        public async Task DeleteAsync(Guid id, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Teacher"))
                throw new Exception("Only teachers can delete courses.");

            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                throw new Exception("Course not found.");

            await _courseRepository.DeleteAsync(id);
        }
    }
}
