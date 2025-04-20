import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TabViewModule } from 'primeng/tabview';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { DividerModule } from 'primeng/divider';

interface Resource {
  id: string;
  title: string;
  type: string;
  description: string;
  downloadUrl?: string;
  fileSize?: string;
  dateAdded: string;
  tags: string[];
  popularity: number;
}

@Component({
  selector: 'vanguard-resources',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    CardModule,
    TabViewModule,
    TagModule,
    DialogModule,
    DividerModule
  ],
  template: `
    <div class="p-8 max-w-7xl mx-auto">
      <div class="flex justify-between items-center mb-8">
        <div>
          <h1 class="text-3xl font-bold" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">
            Teaching Resources
          </h1>
          <p [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
            Tools and materials to enhance your teaching effectiveness
          </p>
        </div>
        <div class="flex gap-2">
          <p-button label="Upload Resource" icon="pi pi-upload"></p-button>
          <p-button label="Request Resource" icon="pi pi-question-circle" styleClass="p-button-outlined"></p-button>
        </div>
      </div>

      <p-tabView>
        <p-tabPanel header="Teaching Materials">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-6">
            <p-card *ngFor="let resource of teachingResources"
              [header]="resource.title"
              styleClass="h-full flex flex-col"
              [ngClass]="{'bg-white': !darkMode, 'bg-gray-900': darkMode}">

              <ng-template pTemplate="header">
                <div class="p-3 text-center"
                  [ngClass]="{'bg-blue-50': !darkMode, 'bg-blue-900': darkMode}">
                  <i class="pi pi-file-pdf text-4xl"
                    [ngClass]="{'text-blue-600': !darkMode, 'text-blue-300': darkMode}"></i>
                </div>
              </ng-template>

              <div class="flex-grow">
                <p class="mb-3" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                  {{resource.description}}
                </p>

                <div class="flex flex-wrap gap-1 mb-3">
                  <p-tag *ngFor="let tag of resource.tags" [value]="tag" styleClass="p-tag-sm"></p-tag>
                </div>

                <div class="flex justify-between items-center text-sm"
                  [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}">
                  <span>Added: {{resource.dateAdded}}</span>
                  <span *ngIf="resource.fileSize">{{resource.fileSize}}</span>
                </div>
              </div>

              <ng-template pTemplate="footer">
                <div class="flex justify-between">
                  <p-button label="Preview" icon="pi pi-eye" styleClass="p-button-outlined"></p-button>
                  <p-button label="Download" icon="pi pi-download"></p-button>
                </div>
              </ng-template>
            </p-card>
          </div>
        </p-tabPanel>

        <p-tabPanel header="Assessment Templates">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-6">
            <p-card *ngFor="let resource of assessmentTemplates"
              [header]="resource.title"
              styleClass="h-full flex flex-col"
              [ngClass]="{'bg-white': !darkMode, 'bg-gray-900': darkMode}">

              <ng-template pTemplate="header">
                <div class="p-3 text-center"
                  [ngClass]="{'bg-green-50': !darkMode, 'bg-green-900': darkMode}">
                  <i class="pi pi-file-excel text-4xl"
                    [ngClass]="{'text-green-600': !darkMode, 'text-green-300': darkMode}"></i>
                </div>
              </ng-template>

              <div class="flex-grow">
                <p class="mb-3" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                  {{resource.description}}
                </p>

                <div class="flex flex-wrap gap-1 mb-3">
                  <p-tag *ngFor="let tag of resource.tags" [value]="tag" styleClass="p-tag-sm"></p-tag>
                </div>

                <div class="flex justify-between items-center text-sm"
                  [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}">
                  <span>Added: {{resource.dateAdded}}</span>
                  <span *ngIf="resource.fileSize">{{resource.fileSize}}</span>
                </div>
              </div>

              <ng-template pTemplate="footer">
                <div class="flex justify-between">
                  <p-button label="Preview" icon="pi pi-eye" styleClass="p-button-outlined"></p-button>
                  <p-button label="Download" icon="pi pi-download"></p-button>
                </div>
              </ng-template>
            </p-card>
          </div>
        </p-tabPanel>

        <p-tabPanel header="Best Practices Guides">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-6">
            <p-card *ngFor="let resource of bestPracticesGuides"
              [header]="resource.title"
              styleClass="h-full flex flex-col"
              [ngClass]="{'bg-white': !darkMode, 'bg-gray-900': darkMode}">

              <ng-template pTemplate="header">
                <div class="p-3 text-center"
                  [ngClass]="{'bg-purple-50': !darkMode, 'bg-purple-900': darkMode}">
                  <i class="pi pi-book text-4xl"
                    [ngClass]="{'text-purple-600': !darkMode, 'text-purple-300': darkMode}"></i>
                </div>
              </ng-template>

              <div class="flex-grow">
                <p class="mb-3" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                  {{resource.description}}
                </p>

                <div class="flex flex-wrap gap-1 mb-3">
                  <p-tag *ngFor="let tag of resource.tags" [value]="tag" styleClass="p-tag-sm"></p-tag>
                </div>

                <div class="flex justify-between items-center text-sm"
                  [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}">
                  <span>Added: {{resource.dateAdded}}</span>
                  <span *ngIf="resource.fileSize">{{resource.fileSize}}</span>
                </div>
              </div>

              <ng-template pTemplate="footer">
                <div class="flex justify-between">
                  <p-button label="Preview" icon="pi pi-eye" styleClass="p-button-outlined"></p-button>
                  <p-button label="Download" icon="pi pi-download"></p-button>
                </div>
              </ng-template>
            </p-card>
          </div>
        </p-tabPanel>
      </p-tabView>

      <!-- Resource Request Section -->
      <div class="mt-10 p-6 rounded-lg" [ngClass]="{'bg-blue-50': !darkMode, 'bg-blue-900': darkMode}">
        <h3 class="text-xl font-bold mb-3" [ngClass]="{'text-blue-800': !darkMode, 'text-blue-100': darkMode}">
          Need Additional Resources?
        </h3>
        <p class="mb-4" [ngClass]="{'text-blue-700': !darkMode, 'text-blue-200': darkMode}">
          Our instructional design team can create custom teaching materials for your courses.
          Request specialized content, assessment templates, or interactive activities.
        </p>
        <p-button label="Submit Resource Request" icon="pi pi-send"></p-button>
      </div>
    </div>
  `
})
export class ResourcesComponent implements OnInit {
  darkMode = false;
  resourceType: string = 'guides'; // Default view

  teachingResources: Resource[] = [
    {
      id: 'r1',
      title: 'Engaging Online Lectures Guide',
      type: 'PDF',
      description: 'Techniques for creating engaging virtual lectures that maintain student attention and participation.',
      fileSize: '2.4 MB',
      dateAdded: 'April 12, 2025',
      tags: ['Online Teaching', 'Lectures'],
      popularity: 87
    },
    {
      id: 'r2',
      title: 'Interactive Coding Exercises',
      type: 'ZIP',
      description: 'A collection of coding exercises with interactive elements to engage students in programming concepts.',
      fileSize: '5.7 MB',
      dateAdded: 'March 28, 2025',
      tags: ['Coding', 'Interactive'],
      popularity: 65
    },
    {
      id: 'r3',
      title: 'Visual Learning Materials for Web Design',
      type: 'PDF',
      description: 'Visual aids and diagrams to explain complex web design concepts and principles.',
      fileSize: '8.2 MB',
      dateAdded: 'April 5, 2025',
      tags: ['Web Design', 'Visual Learning'],
      popularity: 72
    },
    {
      id: 'r4',
      title: 'Database Concept Slides',
      type: 'PPTX',
      description: 'Comprehensive slide deck covering database design, normalization, and query optimization.',
      fileSize: '3.8 MB',
      dateAdded: 'April 15, 2025',
      tags: ['Databases', 'Slides'],
      popularity: 54
    },
    {
      id: 'r5',
      title: 'JavaScript Fundamentals Workbook',
      type: 'PDF',
      description: 'Student workbook with exercises and explanations covering core JavaScript concepts.',
      fileSize: '4.1 MB',
      dateAdded: 'March 10, 2025',
      tags: ['JavaScript', 'Workbook'],
      popularity: 95
    },
    {
      id: 'r6',
      title: 'Group Project Management Tools',
      type: 'ZIP',
      description: 'Templates and tools for effectively managing student group projects and collaboration.',
      fileSize: '1.9 MB',
      dateAdded: 'April 8, 2025',
      tags: ['Project Management', 'Collaboration'],
      popularity: 63
    }
  ];

  assessmentTemplates: Resource[] = [
    {
      id: 'a1',
      title: 'Code Review Rubric',
      type: 'DOCX',
      description: 'Detailed rubric for evaluating student code quality, efficiency, documentation, and functionality.',
      fileSize: '420 KB',
      dateAdded: 'April 10, 2025',
      tags: ['Assessment', 'Coding'],
      popularity: 83
    },
    {
      id: 'a2',
      title: 'Project Presentation Evaluation',
      type: 'XLSX',
      description: 'Spreadsheet template for scoring student project presentations with weighted criteria.',
      fileSize: '350 KB',
      dateAdded: 'March 25, 2025',
      tags: ['Presentations', 'Projects'],
      popularity: 72
    },
    {
      id: 'a3',
      title: 'Database Design Assessment',
      type: 'DOCX',
      description: 'Comprehensive assessment tool for evaluating student database schema designs and normalization.',
      fileSize: '520 KB',
      dateAdded: 'April 7, 2025',
      tags: ['Databases', 'Assessment'],
      popularity: 65
    },
    {
      id: 'a4',
      title: 'Multiple Choice Quiz Generator',
      type: 'XLSX',
      description: 'Excel template that helps generate randomized multiple choice quizzes from question banks.',
      fileSize: '680 KB',
      dateAdded: 'April 14, 2025',
      tags: ['Quizzes', 'Automation'],
      popularity: 91
    },
    {
      id: 'a5',
      title: 'Peer Review Form',
      type: 'DOCX',
      description: 'Structured form for students to provide constructive feedback on peer work and contributions.',
      fileSize: '280 KB',
      dateAdded: 'March 30, 2025',
      tags: ['Peer Review', 'Collaboration'],
      popularity: 78
    },
    {
      id: 'a6',
      title: 'Advanced JavaScript Practical Exam',
      type: 'ZIP',
      description: 'Complete practical examination package with starter code, requirements, and grading criteria.',
      fileSize: '1.2 MB',
      dateAdded: 'April 2, 2025',
      tags: ['JavaScript', 'Exam'],
      popularity: 84
    }
  ];

  bestPracticesGuides: Resource[] = [
    {
      id: 'b1',
      title: 'Effective Online Teaching Strategies',
      type: 'PDF',
      description: 'Comprehensive guide to best practices for engaging students in virtual classrooms.',
      fileSize: '3.2 MB',
      dateAdded: 'April 11, 2025',
      tags: ['Online Teaching', 'Engagement'],
      popularity: 95
    },
    {
      id: 'b2',
      title: 'Providing Constructive Feedback',
      type: 'PDF',
      description: 'Guide to delivering feedback that helps students learn and improve without discouragement.',
      fileSize: '1.8 MB',
      dateAdded: 'March 20, 2025',
      tags: ['Feedback', 'Assessment'],
      popularity: 88
    },
    {
      id: 'b3',
      title: 'Supporting Diverse Learning Styles',
      type: 'PDF',
      description: 'Strategies for adapting teaching methods to accommodate various learning preferences and needs.',
      fileSize: '2.5 MB',
      dateAdded: 'April 5, 2025',
      tags: ['Accessibility', 'Inclusion'],
      popularity: 79
    },
    {
      id: 'b4',
      title: 'Project-Based Learning in Tech Education',
      type: 'PDF',
      description: 'How to design effective project-based learning experiences for technical subjects.',
      fileSize: '4.1 MB',
      dateAdded: 'April 9, 2025',
      tags: ['Project-Based', 'Pedagogy'],
      popularity: 86
    },
    {
      id: 'b5',
      title: 'Student Engagement Techniques',
      type: 'PDF',
      description: 'Research-backed methods to increase student motivation and active participation.',
      fileSize: '2.9 MB',
      dateAdded: 'March 15, 2025',
      tags: ['Engagement', 'Motivation'],
      popularity: 92
    },
    {
      id: 'b6',
      title: 'Building Effective Learning Communities',
      type: 'PDF',
      description: 'How to foster collaboration and peer learning in both online and in-person environments.',
      fileSize: '3.3 MB',
      dateAdded: 'April 3, 2025',
      tags: ['Community', 'Collaboration'],
      popularity: 81
    }
  ];

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    // Get the resource type from the route parameter
    this.route.paramMap.subscribe(params => {
      const type = params.get('type');
      if (type) {
        this.resourceType = type;
      }
    });

    // Check localStorage for dark mode preference
    const storedDarkMode = localStorage.getItem('darkMode');
    if (storedDarkMode) {
      this.darkMode = storedDarkMode === 'true';
    }
  }
}
