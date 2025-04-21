import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { TabViewModule } from 'primeng/tabview';
import { TableModule } from 'primeng/table';
import { ChipModule } from 'primeng/chip';
import { BadgeModule } from 'primeng/badge';
import { TagModule } from 'primeng/tag';

interface Assessment {
  id: string;
  studentName: string;
  courseTitle: string;
  assignmentTitle: string;
  submittedDate: string;
  status: 'pending' | 'graded' | 'late' | 'resubmitted';
}

@Component({
  selector: 'vanguard-assessments',
  standalone: true,
  imports: [CommonModule, ButtonModule, TabViewModule, TableModule, ChipModule, BadgeModule, TagModule],
  template: `
    <div class="p-8 max-w-7xl mx-auto">
      <div class="flex justify-between items-center mb-8">
        <h1 class="text-3xl font-bold" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">
          Student Assessments
        </h1>
        <div class="flex gap-2">
          <p-button label="Create Assignment" icon="pi pi-plus"></p-button>
          <p-button label="Assessment Analytics" icon="pi pi-chart-line" styleClass="p-button-outlined"></p-button>
        </div>
      </div>

      <p-tabView>
        <p-tabPanel header="Pending Reviews ({{pendingAssessments.length}})">
          <div class="p-4 rounded-lg" [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
            <p-table
              [value]="pendingAssessments"
              [paginator]="true"
              [rows]="10"
              [showCurrentPageReport]="true"
              [tableStyle]="{'min-width': '60rem'}"
              [rowHover]="true"
              styleClass="p-datatable-sm">

              <ng-template pTemplate="header">
                <tr>
                  <th>Student</th>
                  <th>Course</th>
                  <th>Assignment</th>
                  <th>Submitted</th>
                  <th>Status</th>
                  <th>Actions</th>
                </tr>
              </ng-template>

              <ng-template pTemplate="body" let-assessment>
                <tr>
                  <td>
                    <div class="font-medium" [ngClass]="{'text-gray-900': !darkMode, 'text-white': darkMode}">
                      {{assessment.studentName}}
                    </div>
                  </td>
                  <td>
                    <span [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                      {{assessment.courseTitle}}
                    </span>
                  </td>
                  <td>
                    <span [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                      {{assessment.assignmentTitle}}
                    </span>
                  </td>
                  <td>
                    <span [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                      {{assessment.submittedDate}}
                    </span>
                  </td>
                  <td>
                    <p-tag
                        [value]="assessment.status"
                        [severity]="assessment.status === 'pending' ? 'warn' :
                          assessment.status === 'graded' ? 'success' :
                          assessment.status === 'resubmitted' ? 'info' : 'danger'">
                    </p-tag>
                  </td>
                  <td>
                    <div class="flex gap-2">
                      <p-button label="Review" icon="pi pi-eye"></p-button>
                    </div>
                  </td>
                </tr>
              </ng-template>

              <ng-template pTemplate="emptymessage">
                <tr>
                  <td colspan="6" class="text-center p-4">
                    <div class="py-8">
                      <i class="pi pi-check-circle text-4xl mb-4 text-green-500"></i>
                      <p [ngClass]="{'text-gray-600': !darkMode, 'text-gray-400': darkMode}">
                        No pending assessments. Great job keeping up!
                      </p>
                    </div>
                  </td>
                </tr>
              </ng-template>
            </p-table>
          </div>
        </p-tabPanel>

        <p-tabPanel header="Graded">
          <div class="p-4 rounded-lg" [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
            <p-table
              [value]="gradedAssessments"
              [paginator]="true"
              [rows]="10"
              [showCurrentPageReport]="true"
              [tableStyle]="{'min-width': '60rem'}"
              [rowHover]="true"
              styleClass="p-datatable-sm">

              <ng-template pTemplate="header">
                <tr>
                  <th>Student</th>
                  <th>Course</th>
                  <th>Assignment</th>
                  <th>Graded</th>
                  <th>Score</th>
                  <th>Actions</th>
                </tr>
              </ng-template>

              <ng-template pTemplate="body" let-assessment>
                <tr>
                  <td>
                    <div class="font-medium" [ngClass]="{'text-gray-900': !darkMode, 'text-white': darkMode}">
                      {{assessment.studentName}}
                    </div>
                  </td>
                  <td>
                    <span [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                      {{assessment.courseTitle}}
                    </span>
                  </td>
                  <td>
                    <span [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                      {{assessment.assignmentTitle}}
                    </span>
                  </td>
                  <td>
                    <span [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                      {{assessment.submittedDate}}
                    </span>
                  </td>
                  <td>
                    <span class="font-bold" [ngClass]="{'text-gray-900': !darkMode, 'text-white': darkMode}">
                      90%
                    </span>
                  </td>
                  <td>
                    <div class="flex gap-2">
                      <p-button label="View" icon="pi pi-eye" styleClass="p-button-outlined"></p-button>
                    </div>
                  </td>
                </tr>
              </ng-template>
            </p-table>
          </div>
        </p-tabPanel>

        <p-tabPanel header="All Assignments">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div class="p-4 rounded-lg" [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
              <div class="flex justify-between items-start mb-4">
                <div>
                  <h3 class="text-lg font-semibold" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">
                    Final Project
                  </h3>
                  <p class="text-sm" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                    Web Development Fundamentals
                  </p>
                </div>
                <p-badge value="32" severity="info"></p-badge>
              </div>
              <p class="mb-4" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                Build a responsive website with HTML, CSS and JavaScript that demonstrates core web development principles.
              </p>
              <div class="flex justify-between items-center">
                <span class="text-sm" [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}">
                  Due: May 15, 2025
                </span>
                <p-button label="Manage" icon="pi pi-cog" styleClass="p-button-sm"></p-button>
              </div>
            </div>

            <div class="p-4 rounded-lg" [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
              <div class="flex justify-between items-start mb-4">
                <div>
                  <h3 class="text-lg font-semibold" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">
                    Database Design
                  </h3>
                  <p class="text-sm" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                    Advanced JavaScript Concepts
                  </p>
                </div>
                <p-badge value="18" severity="info"></p-badge>
              </div>
              <p class="mb-4" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                Create a normalized database schema and implement it with MongoDB and Mongoose.
              </p>
              <div class="flex justify-between items-center">
                <span class="text-sm" [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}">
                  Due: April 30, 2025
                </span>
                <p-button label="Manage" icon="pi pi-cog" styleClass="p-button-sm"></p-button>
              </div>
            </div>

            <div class="p-4 rounded-lg" [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
              <div class="flex justify-between items-start mb-4">
                <div>
                  <h3 class="text-lg font-semibold" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">
                    Authentication System
                  </h3>
                  <p class="text-sm" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                    Full Stack Web Development
                  </p>
                </div>
                <p-badge value="26" severity="info"></p-badge>
              </div>
              <p class="mb-4" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                Implement a secure authentication system with JWT tokens, password hashing, and role-based access control.
              </p>
              <div class="flex justify-between items-center">
                <span class="text-sm" [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}">
                  Due: May 5, 2025
                </span>
                <p-button label="Manage" icon="pi pi-cog" styleClass="p-button-sm"></p-button>
              </div>
            </div>
          </div>
        </p-tabPanel>
      </p-tabView>

      <!-- Quick Tips Section -->
      <div class="mt-8 p-6 rounded-lg" [ngClass]="{'bg-purple-50': !darkMode, 'bg-purple-900': darkMode}">
        <h3 class="text-xl font-semibold mb-3" [ngClass]="{'text-purple-800': !darkMode, 'text-purple-100': darkMode}">
          Assessment Best Practices
        </h3>
        <ul class="list-disc pl-5 space-y-2 mb-4" [ngClass]="{'text-purple-700': !darkMode, 'text-purple-200': darkMode}">
          <li>Provide specific, actionable feedback on all submissions</li>
          <li>Use rubrics to ensure consistent grading</li>
          <li>Grade assignments within 48 hours of submission</li>
          <li>Encourage students to review feedback and ask questions</li>
          <li>Use a variety of assessment types to evaluate different skills</li>
        </ul>
        <p-button label="Download Assessment Templates" icon="pi pi-download" styleClass="p-button-outlined"></p-button>
      </div>
    </div>
  `
})
export class AssessmentsComponent implements OnInit {
  darkMode = false;

  pendingAssessments: Assessment[] = [
    {
      id: 'a1',
      studentName: 'Alex Johnson',
      courseTitle: 'Web Development Fundamentals',
      assignmentTitle: 'Final Project',
      submittedDate: 'Yesterday',
      status: 'pending'
    },
    {
      id: 'a2',
      studentName: 'Maria Garcia',
      courseTitle: 'Advanced JavaScript Concepts',
      assignmentTitle: 'Async Programming Challenge',
      submittedDate: '2 days ago',
      status: 'pending'
    },
    {
      id: 'a3',
      studentName: 'David Kim',
      courseTitle: 'Full Stack Web Development',
      assignmentTitle: 'React Component Library',
      submittedDate: 'Today',
      status: 'resubmitted'
    },
    {
      id: 'a4',
      studentName: 'Sarah Wilson',
      courseTitle: 'Web Development Fundamentals',
      assignmentTitle: 'CSS Layout Challenge',
      submittedDate: '3 days ago',
      status: 'late'
    },
    {
      id: 'a5',
      studentName: 'Michael Brown',
      courseTitle: 'Mobile App Development',
      assignmentTitle: 'UI Prototype',
      submittedDate: 'Yesterday',
      status: 'pending'
    }
  ];

  gradedAssessments: Assessment[] = [
    {
      id: 'a6',
      studentName: 'Jennifer Lee',
      courseTitle: 'Web Development Fundamentals',
      assignmentTitle: 'HTML Basics Quiz',
      submittedDate: 'Last week',
      status: 'graded'
    },
    {
      id: 'a7',
      studentName: 'Robert Martinez',
      courseTitle: 'Advanced JavaScript Concepts',
      assignmentTitle: 'Functional Programming',
      submittedDate: '2 weeks ago',
      status: 'graded'
    },
    {
      id: 'a8',
      studentName: 'Emily Chen',
      courseTitle: 'Full Stack Web Development',
      assignmentTitle: 'Database Design',
      submittedDate: '3 days ago',
      status: 'graded'
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
