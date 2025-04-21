import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CarouselModule } from 'primeng/carousel';
import { Router } from '@angular/router';

interface Session {
    id: string;
    date: string;
    title: string;
    description: string;
    time: string;
    students: number;
}

interface TeacherStats {
    activeCourses: number;
    totalStudents: number;
    pendingAssignments: number;
    averageRating: number;
}

@Component({
    selector: 'vanguard-dashboard',
    standalone: true,
    imports: [CommonModule, ButtonModule, CarouselModule],
    template: `
    <div class="flex-1 flex flex-col items-center py-10 px-4 sm:px-6 lg:px-8">
      <!-- Stats Section -->
      <div class="w-full max-w-7xl mx-auto mb-10">
        <h2 class="text-2xl font-bold mb-6" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">
          Teacher Dashboard
        </h2>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div class="p-6 rounded-lg text-center"
            [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
            <h4 class="text-5xl font-bold mb-2" [ngClass]="{'text-blue-600': !darkMode, 'text-blue-400': darkMode}">
              {{stats.activeCourses}}</h4>
            <p class="text-lg" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">Active Courses</p>
          </div>
          <div class="p-6 rounded-lg text-center"
            [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
            <h4 class="text-5xl font-bold mb-2" [ngClass]="{'text-green-600': !darkMode, 'text-green-400': darkMode}">
              {{stats.totalStudents}}</h4>
            <p class="text-lg" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">Enrolled Students</p>
          </div>
          <div class="p-6 rounded-lg text-center"
            [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
            <h4 class="text-5xl font-bold mb-2" [ngClass]="{'text-yellow-600': !darkMode, 'text-yellow-400': darkMode}">
              {{stats.pendingAssignments}}</h4>
            <p class="text-lg" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">Pending Reviews</p>
          </div>
          <div class="p-6 rounded-lg text-center"
            [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
            <h4 class="text-5xl font-bold mb-2" [ngClass]="{'text-purple-600': !darkMode, 'text-purple-400': darkMode}">
              {{stats.averageRating.toFixed(1)}}</h4>
            <p class="text-lg" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">Avg. Rating</p>
          </div>
        </div>
      </div>

      <!-- Upcoming Sessions -->
      <div class="w-full max-w-7xl mx-auto mb-10 p-6 rounded-lg"
        [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
        <h3 class="text-2xl font-bold mb-6" [ngClass]="{'text-gray-800': !darkMode, 'text-gray-100': darkMode}">Upcoming
          Sessions</h3>

        <p-carousel [value]="upcomingSessions" [numVisible]="1" [numScroll]="1" [circular]="true"
          [responsiveOptions]="carouselResponsiveOptions">
          <ng-template let-session pTemplate="item">
            <div class="p-4 border rounded-lg m-2"
              [ngClass]="{'border-gray-200': !darkMode, 'border-gray-700': darkMode}">
              <div class="flex items-start gap-4">
                <div class="flex-shrink-0 w-16 h-16 rounded-full flex items-center justify-center"
                  [ngClass]="{'bg-blue-100 text-blue-700': !darkMode, 'bg-blue-900 text-blue-200': darkMode}">
                  <span class="text-2xl font-bold">{{session.date}}</span>
                </div>
                <div class="flex-1">
                  <h4 class="font-medium text-xl" [ngClass]="{'text-gray-900': !darkMode, 'text-white': darkMode}">
                    {{session.title}}</h4>
                  <p class="mt-1" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                    {{session.description}}</p>
                  <div class="mt-3 flex items-center gap-2">
                    <i class="pi pi-clock" [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}"></i>
                    <span [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}">{{session.time}}</span>
                    <i class="pi pi-users ml-4" [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}"></i>
                    <span [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}">{{session.students}}
                      Students</span>
                  </div>
                </div>
                <p-button icon="pi pi-video" styleClass="p-button-rounded"
                  (onClick)="startSession(session.id)"></p-button>
              </div>
            </div>
          </ng-template>
        </p-carousel>
      </div>

      <!-- Features Grid -->
      <div class="max-w-7xl w-full mx-auto grid md:grid-cols-3 gap-6 mb-12">
        <!-- Course Management -->
        <div class="p-6 rounded-lg h-full flex flex-col"
          [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
          <div class="flex justify-center mb-4">
            <i class="pi pi-folder-open text-5xl" [ngClass]="{'text-blue-500': !darkMode, 'text-blue-400': darkMode}"></i>
          </div>
          <h3 class="text-xl font-semibold text-center mb-3"
            [ngClass]="{'text-gray-800': !darkMode, 'text-gray-100': darkMode}">Course Management</h3>
          <p class="text-center mb-4 flex-grow" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
            Create, edit, and publish courses with our intuitive content management system.
          </p>
          <p-button label="Manage Courses" styleClass="p-button-outlined w-full"
            (onClick)="navigateTo('/courses')"></p-button>
        </div>

        <!-- Student Assessment -->
        <div class="p-6 rounded-lg h-full flex flex-col"
          [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
          <div class="flex justify-center mb-4">
            <i class="pi pi-chart-bar text-5xl" [ngClass]="{'text-green-500': !darkMode, 'text-green-400': darkMode}"></i>
          </div>
          <h3 class="text-xl font-semibold text-center mb-3"
            [ngClass]="{'text-gray-800': !darkMode, 'text-gray-100': darkMode}">Student Assessment</h3>
          <p class="text-center mb-4 flex-grow" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
            Track student progress, grade assignments, and provide timely feedback to enhance learning.
          </p>
          <p-button label="View Assessments" styleClass="p-button-outlined w-full"
            (onClick)="navigateTo('/assessments')"></p-button>
        </div>

        <!-- Classroom Discussions -->
        <div class="p-6 rounded-lg h-full flex flex-col"
          [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
          <div class="flex justify-center mb-4">
            <i class="pi pi-comments text-5xl"
              [ngClass]="{'text-purple-500': !darkMode, 'text-purple-400': darkMode}"></i>
          </div>
          <h3 class="text-xl font-semibold text-center mb-3"
            [ngClass]="{'text-gray-800': !darkMode, 'text-gray-100': darkMode}">Classroom Discussions</h3>
          <p class="text-center mb-4 flex-grow" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
            Host live discussions, answer questions, and foster a collaborative learning environment.
          </p>
          <p-button label="Engage Students" styleClass="p-button-outlined w-full"
            (onClick)="navigateTo('/discussions')"></p-button>
        </div>
      </div>

      <!-- Resource Section -->
      <div class="text-center w-full max-w-3xl mx-auto p-8 rounded-lg mb-10"
        [ngClass]="{'bg-blue-50': !darkMode, 'bg-blue-900': darkMode}">
        <h3 class="text-2xl font-bold mb-4" [ngClass]="{'text-blue-800': !darkMode, 'text-blue-100': darkMode}">Teaching
          Resources</h3>
        <p class="mb-6" [ngClass]="{'text-blue-600': !darkMode, 'text-blue-200': darkMode}">
          Access teaching materials, best practices, and tools to enhance your course delivery and student engagement.
        </p>
        <div class="flex flex-wrap justify-center gap-4">
          <p-button label="Teaching Guides" icon="pi pi-book" styleClass="p-button-raised"
            (onClick)="navigateTo('/resources/guides')"></p-button>
          <p-button label="Assignment Templates" icon="pi pi-file" styleClass="p-button-outlined"
            (onClick)="navigateTo('/resources/templates')"></p-button>
          <p-button label="Community Forum" icon="pi pi-users" styleClass="p-button-outlined"
            (onClick)="navigateTo('/resources/forum')"></p-button>
        </div>
      </div>
    </div>
  `
})
export class DashboardComponent implements OnInit {
    darkMode = false;

    stats: TeacherStats = {
        activeCourses: 5,
        totalStudents: 127,
        pendingAssignments: 23,
        averageRating: 4.8
    };

    upcomingSessions: Session[] = [
        {
            id: 'session1',
            date: '23',
            title: 'Web Development Fundamentals',
            description: 'Introduction to HTML5 and CSS3 core concepts',
            time: 'Today at 2:00 PM',
            students: 24
        },
        {
            id: 'session2',
            date: '25',
            title: 'Database Design Workshop',
            description: 'Hands-on session on relational database design principles',
            time: 'Wednesday at 10:00 AM',
            students: 18
        },
        {
            id: 'session3',
            date: '26',
            title: 'JavaScript Advanced Concepts',
            description: 'Deep dive into closures, promises, and async programming',
            time: 'Thursday at 3:30 PM',
            students: 21
        },
        {
            id: 'session4',
            date: '29',
            title: 'Project Presentations',
            description: 'Final project presentations and peer feedback session',
            time: 'Monday at 1:00 PM',
            students: 32
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

    constructor(private router: Router) { }

    ngOnInit() {
        // Check localStorage for dark mode preference
        const storedDarkMode = localStorage.getItem('darkMode');
        if (storedDarkMode) {
            this.darkMode = storedDarkMode === 'true';
        }
    }

    navigateTo(route: string) {
        this.router.navigate([route]);
    }

    startSession(sessionId: string) {
        console.log(`Starting session ${sessionId}`);
        this.navigateTo(`/classroom/${sessionId}`);
    }
}
