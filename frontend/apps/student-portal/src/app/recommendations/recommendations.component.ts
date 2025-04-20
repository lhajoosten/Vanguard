import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { RatingModule } from 'primeng/rating';
import { FormsModule } from '@angular/forms';

interface Recommendation {
  id: number;
  title: string;
  description: string;
  rating: number;
  instructor: string;
  duration: string;
  level: string;
  image: string;
}

@Component({
  selector: 'vanguard-recommendations',
  standalone: true,
  imports: [CommonModule, CardModule, ButtonModule, RatingModule, FormsModule],
  template: `
    <div class="py-10 px-4 sm:px-6 lg:px-8">
      <div class="max-w-7xl mx-auto">
        <h1 class="text-3xl font-bold mb-8" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">
          Recommended Courses for You
        </h1>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div *ngFor="let course of recommendations" class="col-span-1">
            <p-card
              [ngClass]="{'bg-white': !darkMode, 'bg-gray-800': darkMode}"
              styleClass="p-card-shadow">
              <ng-template pTemplate="header">
                <img alt="Course Image" [src]="course.image" class="w-full h-48 object-cover"/>
              </ng-template>

              <h3 class="text-xl font-bold mb-2" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">
                {{course.title}}
              </h3>

              <div class="flex items-center mb-2">
                <p-rating [(ngModel)]="course.rating" [readonly]="true"></p-rating>
                <span class="ml-2" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                  {{course.rating.toFixed(1)}}
                </span>
              </div>

              <p class="mb-3" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                {{course.description}}
              </p>

              <div class="flex flex-wrap gap-2 mb-3">
                <span class="px-2 py-1 text-xs rounded"
                  [ngClass]="{'bg-blue-100 text-blue-800': !darkMode, 'bg-blue-900 text-blue-100': darkMode}">
                  {{course.level}}
                </span>
                <span class="px-2 py-1 text-xs rounded"
                  [ngClass]="{'bg-green-100 text-green-800': !darkMode, 'bg-green-900 text-green-100': darkMode}">
                  {{course.duration}}
                </span>
              </div>

              <p class="text-sm mb-4" [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}">
                Instructor: {{course.instructor}}
              </p>

              <ng-template pTemplate="footer">
                <div class="flex justify-between">
                  <p-button label="Enroll Now" styleClass="p-button-raised"></p-button>
                  <p-button label="Add to Wishlist" styleClass="p-button-outlined"></p-button>
                </div>
              </ng-template>
            </p-card>
          </div>
        </div>
      </div>
    </div>
  `
})
export class RecommendationsComponent implements OnInit {
  darkMode = false;

  recommendations: Recommendation[] = [
    {
      id: 1,
      title: "Full Stack Web Development",
      description: "Learn to build complete web applications with front and back-end technologies.",
      rating: 4.8,
      instructor: "Dr. Sarah Williams",
      duration: "12 weeks",
      level: "Intermediate",
      image: "assets/courses/web-dev.jpg"
    },
    {
      id: 2,
      title: "Cloud Architecture Fundamentals",
      description: "Master cloud infrastructure design principles and best practices.",
      rating: 4.6,
      instructor: "Michael Chen",
      duration: "8 weeks",
      level: "Advanced",
      image: "assets/courses/cloud.jpg"
    },
    {
      id: 3,
      title: "Data Science with Python",
      description: "Apply data analysis techniques using Python libraries like NumPy and Pandas.",
      rating: 4.9,
      instructor: "Dr. Alicia Rodriguez",
      duration: "10 weeks",
      level: "Intermediate",
      image: "assets/courses/data-science.jpg"
    },
    {
      id: 4,
      title: "Mobile App Development",
      description: "Build native mobile applications for iOS and Android platforms.",
      rating: 4.7,
      instructor: "James Wilson",
      duration: "14 weeks",
      level: "Intermediate",
      image: "assets/courses/mobile-dev.jpg"
    },
    {
      id: 5,
      title: "DevOps Engineering",
      description: "Learn CI/CD pipelines, containerization, and infrastructure as code.",
      rating: 4.5,
      instructor: "Emily Chang",
      duration: "9 weeks",
      level: "Advanced",
      image: "assets/courses/devops.jpg"
    },
    {
      id: 6,
      title: "Cybersecurity Fundamentals",
      description: "Understand security principles and implement protection strategies.",
      rating: 4.7,
      instructor: "Robert Smith",
      duration: "11 weeks",
      level: "Beginner",
      image: "assets/courses/security.jpg"
    }
  ];

  ngOnInit() {
    // Check localStorage for dark mode preference
    const storedDarkMode = localStorage.getItem('darkMode');
    if (storedDarkMode) {
      this.darkMode = storedDarkMode === 'true';
    }
  }
}
