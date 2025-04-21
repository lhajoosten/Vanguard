import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { Router } from '@angular/router';

interface Course {
  id: string;
  title: string;
  level: string;
  students: number;
  progress: number;
  status: 'active' | 'draft' | 'archived';
  lastUpdated: string;
}

@Component({
  selector: 'vanguard-courses',
  standalone: true,
  imports: [CommonModule, ButtonModule, TableModule, TagModule, TooltipModule],
  template: `
    <div class="p-8 max-w-7xl mx-auto">
      <div class="flex justify-between items-center mb-8">
        <h1 class="text-3xl font-bold" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">Course Management</h1>
        <p-button label="Create New Course" icon="pi pi-plus"></p-button>
      </div>

      <div class="mb-8 p-4 rounded-lg" [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
        <p-table 
          [value]="courses" 
          [paginator]="true" 
          [rows]="5"
          [showCurrentPageReport]="true" 
          [tableStyle]="{'min-width': '60rem'}"
          [rowHover]="true"
          currentPageReportTemplate="Showing {first} to {last} of {totalRecords} courses"
          [rowsPerPageOptions]="[5, 10, 25]"
          styleClass="p-datatable-sm"
          [scrollable]="true">
          
          <ng-template pTemplate="header">
            <tr>
              <th pSortableColumn="title">Course <p-sortIcon field="title"></p-sortIcon></th>
              <th pSortableColumn="level">Level <p-sortIcon field="level"></p-sortIcon></th>
              <th pSortableColumn="students">Students <p-sortIcon field="students"></p-sortIcon></th>
              <th pSortableColumn="progress">Progress <p-sortIcon field="progress"></p-sortIcon></th>
              <th pSortableColumn="status">Status <p-sortIcon field="status"></p-sortIcon></th>
              <th pSortableColumn="lastUpdated">Last Updated <p-sortIcon field="lastUpdated"></p-sortIcon></th>
              <th>Actions</th>
            </tr>
          </ng-template>
          
          <ng-template pTemplate="body" let-course>
            <tr>
              <td>
                <div class="font-medium" [ngClass]="{'text-gray-900': !darkMode, 'text-white': darkMode}">
                  {{course.title}}
                </div>
              </td>
              <td>
                <span [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">{{course.level}}</span>
              </td>
              <td>
                <span [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">{{course.students}}</span>
              </td>
              <td>
                <div class="w-full bg-gray-200 rounded-full h-2.5 dark:bg-gray-700">
                  <div class="bg-blue-600 h-2.5 rounded-full" [style.width]="course.progress + '%'"></div>
                </div>
                <span class="text-xs" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                  {{course.progress}}% Complete
                </span>
              </td>
              <td>
                <p-tag [value]="course.status" 
                  [severity]="course.status === 'active' ? 'success' : course.status === 'draft' ? 'warn' : 'info'">
                </p-tag>
              </td>
              <td>
                <span [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">{{course.lastUpdated}}</span>
              </td>
              <td>
                <div class="flex gap-2">
                  <button 
                    pButton 
                    pRipple 
                    icon="pi pi-pencil" 
                    class="p-button-rounded p-button-text"
                    pTooltip="Edit Course" 
                    tooltipPosition="top">
                  </button>
                  <button 
                    pButton 
                    pRipple 
                    icon="pi pi-eye" 
                    class="p-button-rounded p-button-text p-button-secondary"
                    pTooltip="Preview Course" 
                    tooltipPosition="top">
                  </button>
                  <button 
                    pButton 
                    pRipple 
                    icon="pi pi-users" 
                    class="p-button-rounded p-button-text p-button-info"
                    pTooltip="Manage Students" 
                    tooltipPosition="top">
                  </button>
                  <button 
                    pButton 
                    pRipple 
                    icon="pi pi-ellipsis-v" 
                    class="p-button-rounded p-button-text"
                    pTooltip="More Options" 
                    tooltipPosition="top">
                  </button>
                </div>
              </td>
            </tr>
          </ng-template>
          
          <ng-template pTemplate="emptymessage">
            <tr>
              <td colspan="7" class="text-center p-4">
                <div class="py-8">
                  <i class="pi pi-folder-open text-4xl mb-4" [ngClass]="{'text-gray-400': !darkMode, 'text-gray-600': darkMode}"></i>
                  <p [ngClass]="{'text-gray-600': !darkMode, 'text-gray-400': darkMode}">No courses found.</p>
                  <p-button label="Create Your First Course" icon="pi pi-plus" styleClass="mt-4"></p-button>
                </div>
              </td>
            </tr>
          </ng-template>
        </p-table>
      </div>

      <!-- Course Creation Tips -->
      <div class="rounded-lg p-6" [ngClass]="{'bg-blue-50': !darkMode, 'bg-blue-900': darkMode}">
        <h3 class="text-xl font-semibold mb-3" [ngClass]="{'text-blue-800': !darkMode, 'text-blue-100': darkMode}">
          Course Creation Best Practices
        </h3>
        <ul class="list-disc pl-5 space-y-2 mb-4" [ngClass]="{'text-blue-700': !darkMode, 'text-blue-200': darkMode}">
          <li>Create clear learning objectives for each module</li>
          <li>Include a variety of content formats (video, text, interactive)</li>
          <li>Design assessments that align with learning objectives</li>
          <li>Provide opportunities for student collaboration</li>
          <li>Include regular feedback mechanisms throughout the course</li>
        </ul>
        <p-button label="Read Course Design Guide" icon="pi pi-book" styleClass="p-button-outlined"></p-button>
      </div>
    </div>
  `
})
export class CoursesComponent implements OnInit {
  darkMode = false;

  courses: Course[] = [
    {
      id: 'course1',
      title: 'Web Development Fundamentals',
      level: 'Beginner',
      students: 86,
      progress: 100,
      status: 'active',
      lastUpdated: '2 days ago'
    },
    {
      id: 'course2',
      title: 'Advanced JavaScript Concepts',
      level: 'Intermediate',
      students: 42,
      progress: 75,
      status: 'active',
      lastUpdated: '1 week ago'
    },
    {
      id: 'course3',
      title: 'Full Stack Web Development with React and Node',
      level: 'Advanced',
      students: 34,
      progress: 60,
      status: 'active',
      lastUpdated: '3 days ago'
    },
    {
      id: 'course4',
      title: 'Mobile App Development with Flutter',
      level: 'Intermediate',
      students: 29,
      progress: 40,
      status: 'draft',
      lastUpdated: 'Yesterday'
    },
    {
      id: 'course5',
      title: 'Introduction to Python Programming',
      level: 'Beginner',
      students: 56,
      progress: 90,
      status: 'active',
      lastUpdated: '4 days ago'
    },
    {
      id: 'course6',
      title: 'Cybersecurity Fundamentals',
      level: 'Intermediate',
      students: 38,
      progress: 85,
      status: 'active',
      lastUpdated: '1 day ago'
    },
    {
      id: 'course7',
      title: 'Cloud Computing and DevOps',
      level: 'Advanced',
      students: 22,
      progress: 25,
      status: 'draft',
      lastUpdated: '5 days ago'
    },
    {
      id: 'course8',
      title: 'Data Science with Python',
      level: 'Intermediate',
      students: 0,
      progress: 10,
      status: 'draft',
      lastUpdated: 'Just now'
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
}