import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CarouselModule } from 'primeng/carousel';

interface Activity {
  avatar: string;
  title: string;
  description: string;
  time: string;
}

@Component({
  selector: 'vanguard-dashboard',
  standalone: true,
  imports: [CommonModule, ButtonModule, CarouselModule],
  template: `
    <div class="flex-1 flex flex-col justify-center items-center py-10 px-4 sm:px-6 lg:px-8">
      <div class="w-full max-w-7xl mx-auto mb-10 p-6 rounded-lg"
        [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
        <h2 class="text-2xl font-bold mb-6" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">
          Recent Activity
        </h2>

        <p-carousel [value]="recentActivities" [numVisible]="1" [numScroll]="1" [circular]="true"
          [responsiveOptions]="carouselResponsiveOptions">
          <ng-template let-activity pTemplate="item">
            <div class="p-4 border rounded-lg m-2"
              [ngClass]="{'bg-white border-gray-200': !darkMode, 'bg-gray-800 border-gray-700': darkMode}">
              <div class="flex items-start gap-4">
                <div>
                  <h4 class="font-medium" [ngClass]="{'text-gray-900': !darkMode, 'text-white': darkMode}">
                    {{activity.title}}</h4>
                  <p class="mt-1" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                    {{activity.description}}</p>
                  <p class="mt-2 text-sm" [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}">
                    {{activity.time}}</p>
                </div>
              </div>
            </div>
          </ng-template>
        </p-carousel>
      </div>

      <!-- Call to Action -->
      <div class="text-center w-full max-w-3xl mx-auto p-8 rounded-lg mb-10"
        [ngClass]="{'bg-blue-50': !darkMode, 'bg-blue-900': darkMode}">
        <h3 class="text-2xl font-bold mb-4" [ngClass]="{'text-blue-800': !darkMode, 'text-blue-100': darkMode}">
          Course Enrollment
        </h3>
        <p class="mb-6" [ngClass]="{'text-blue-600': !darkMode, 'text-blue-200': darkMode}">
          Join thousands of students who are advancing their careers through Vanguard's personalized learning paths.
        </p>
        <div class="flex justify-center gap-4">
          <p-button label="My Courses" styleClass="p-button-raised"></p-button>
          <p-button label="Recommended Courses" styleClass="p-button-outlined"></p-button>
        </div>
      </div>
    </div>
  `
})
export class DashboardComponent implements OnInit {
  darkMode = false;

  recentActivities: Activity[] = [
    {
      avatar: 'assets/avatars/user1.jpg',
      title: 'Maria J. completed "Advanced Data Analysis"',
      description: 'Earned a certification with distinction after completing all modules',
      time: '2 hours ago'
    },
    {
      avatar: 'assets/avatars/user2.jpg',
      title: 'Alex K. started a new discussion',
      description: 'Topic: "Best practices for frontend development in 2025"',
      time: '4 hours ago'
    },
    {
      avatar: 'assets/avatars/user3.jpg',
      title: 'Sam T. achieved a skill milestone',
      description: 'Reached "Advanced" level in Full Stack Development',
      time: 'Yesterday'
    },
    {
      avatar: 'assets/avatars/user4.jpg',
      title: 'New course available',
      description: '"Cloud Architecture Fundamentals" is now open for enrollment',
      time: '2 days ago'
    }
  ];

  carouselResponsiveOptions = [
    {
      breakpoint: '1024px',
      numVisible: 1,
      numScroll: 1
    },
    {
      breakpoint: '768px',
      numVisible: 1,
      numScroll: 1
    },
    {
      breakpoint: '560px',
      numVisible: 1,
      numScroll: 1
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
