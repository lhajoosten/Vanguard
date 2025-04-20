import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { AvatarModule } from 'primeng/avatar';
import { ChipModule } from 'primeng/chip';
import { FormsModule } from '@angular/forms';
import { DividerModule } from 'primeng/divider';

interface Discussion {
  id: string;
  title: string;
  course: string;
  creator: string;
  creatorAvatar: string;
  date: string;
  replies: number;
  lastActivity: string;
  tags: string[];
}

interface Message {
  id: string;
  user: string;
  avatar: string;
  content: string;
  timestamp: string;
  replies?: Message[];
}

@Component({
  selector: 'vanguard-discussions',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    CardModule,
    AvatarModule,
    ChipModule,
    FormsModule,
    DividerModule
  ],
  template: `
    <div class="p-8 max-w-7xl mx-auto">
      <div class="flex justify-between items-center mb-8">
        <h1 class="text-3xl font-bold" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">
          Class Discussions
        </h1>
        <div class="flex gap-2">
          <p-button label="Create Topic" icon="pi pi-plus"></p-button>
          <p-button label="Announcements" icon="pi pi-megaphone" styleClass="p-button-outlined"></p-button>
        </div>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-12 gap-6">
        <!-- Active Discussions (Left Column) -->
        <div class="md:col-span-4 space-y-4">
          <h2 class="text-xl font-bold mb-4" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">
            Active Discussions
          </h2>

          <div *ngFor="let discussion of discussions"
            class="p-4 rounded-lg cursor-pointer transition-colors"
            [ngClass]="{'bg-white shadow hover:bg-gray-50': !darkMode, 'bg-gray-900 shadow-lg hover:bg-gray-800': darkMode}">
            <div class="flex items-start gap-3">
              <p-avatar [image]="discussion.creatorAvatar" shape="circle" size="large"></p-avatar>
              <div class="flex-1">
                <h3 class="font-medium mb-1" [ngClass]="{'text-gray-900': !darkMode, 'text-white': darkMode}">
                  {{discussion.title}}
                </h3>
                <p class="text-sm mb-2" [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                  {{discussion.course}}
                </p>
                <div class="flex flex-wrap gap-1 mb-2">
                  <p-chip *ngFor="let tag of discussion.tags" [label]="tag" styleClass="text-xs p-chip-sm"></p-chip>
                </div>
                <div class="flex items-center justify-between text-xs mt-2">
                  <span [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}">
                    {{discussion.replies}} replies
                  </span>
                  <span [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}">
                    Last activity: {{discussion.lastActivity}}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Current Discussion (Right Column) -->
        <div class="md:col-span-8">
          <div class="p-6 rounded-lg mb-6"
            [ngClass]="{'bg-white shadow': !darkMode, 'bg-gray-900 shadow-lg': darkMode}">
            <div class="flex justify-between items-start mb-6">
              <div>
                <h2 class="text-2xl font-bold mb-2" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">
                  Challenges with Asynchronous JavaScript
                </h2>
                <div class="flex items-center gap-2">
                  <p-avatar image="assets/avatars/student3.jpg" shape="circle"></p-avatar>
                  <span [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
                    Started by Sarah Wilson • Advanced JavaScript Concepts
                  </span>
                </div>
              </div>
              <p-button icon="pi pi-pin" styleClass="p-button-rounded p-button-text"></p-button>
            </div>

            <div class="space-y-6">
              <!-- Original post -->
              <div class="p-4 rounded-lg"
                [ngClass]="{'bg-blue-50': !darkMode, 'bg-blue-900': darkMode}">
                <p [ngClass]="{'text-gray-800': !darkMode, 'text-blue-100': darkMode}">
                  I'm having trouble understanding the difference between Promises and async/await.
                  Could someone explain when to use one over the other? Also, I'm running into an issue
                  where my async functions are executing out of order. Any tips would be helpful!
                </p>
                <div class="flex justify-between items-center mt-4">
                  <span class="text-sm" [ngClass]="{'text-gray-600': !darkMode, 'text-blue-200': darkMode}">
                    Posted 2 days ago
                  </span>
                  <div class="flex gap-2">
                    <p-button icon="pi pi-reply" label="Reply" styleClass="p-button-sm"></p-button>
                  </div>
                </div>
              </div>

              <!-- Replies -->
              <div *ngFor="let message of messages" class="pl-4 border-l-2 border-blue-300">
                <div class="p-4 rounded-lg"
                  [ngClass]="{'bg-gray-50': !darkMode, 'bg-gray-800': darkMode}">
                  <div class="flex items-center gap-2 mb-2">
                    <p-avatar [image]="message.avatar" shape="circle"></p-avatar>
                    <div>
                      <span class="font-medium" [ngClass]="{'text-gray-900': !darkMode, 'text-white': darkMode}">
                        {{message.user}}
                      </span>
                      <span class="text-xs ml-2" [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}">
                        {{message.timestamp}}
                      </span>
                    </div>
                  </div>
                  <p class="mb-3" [ngClass]="{'text-gray-800': !darkMode, 'text-gray-200': darkMode}">
                    {{message.content}}
                  </p>
                  <div class="flex justify-end gap-2">
                    <p-button icon="pi pi-thumbs-up" styleClass="p-button-text p-button-sm"></p-button>
                    <p-button icon="pi pi-reply" styleClass="p-button-text p-button-sm"></p-button>
                  </div>
                </div>

                <!-- Nested replies -->
                                <div *ngFor="let reply of message.replies" class="pl-4 mt-2 border-l-2 border-gray-300">
                                  <div class="p-4 rounded-lg"
                                    [ngClass]="{'bg-gray-50': !darkMode, 'bg-gray-800': darkMode}">
                                    <div class="flex items-center gap-2 mb-2">
                                      <p-avatar [image]="reply.avatar" shape="circle" size="normal"></p-avatar>
                                      <div>
                        <span class="font-medium" [ngClass]="{'text-gray-900': !darkMode, 'text-white': darkMode}">
                          {{reply.user}}
                        </span>
                        <span class="text-xs ml-2" [ngClass]="{'text-gray-500': !darkMode, 'text-gray-400': darkMode}">
                          {{reply.timestamp}}
                        </span>
                      </div>
                    </div>
                    <p [ngClass]="{'text-gray-800': !darkMode, 'text-gray-200': darkMode}">
                      {{reply.content}}
                    </p>
                  </div>
                </div>
              </div>

              <!-- Reply box -->
              <div class="mt-6">
                <p-divider></p-divider>
                <h3 class="text-lg font-medium mb-2" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">
                  Your Response
                </h3>
                <textarea pInputTextarea rows="3" class="w-full mb-3" placeholder="Type your response here..."></textarea>
                <div class="flex justify-end">
                  <p-button label="Post Reply" icon="pi pi-send"></p-button>
                </div>
              </div>
            </div>
          </div>

          <!-- Discussion Tips -->
          <div class="p-4 rounded-lg" [ngClass]="{'bg-green-50': !darkMode, 'bg-green-900': darkMode}">
            <h3 class="text-lg font-medium mb-2" [ngClass]="{'text-green-800': !darkMode, 'text-green-100': darkMode}">
              Fostering Engaging Discussions
            </h3>
            <ul class="list-disc pl-5 space-y-1" [ngClass]="{'text-green-700': !darkMode, 'text-green-200': darkMode}">
              <li>Ask open-ended questions to encourage deeper thinking</li>
              <li>Recognize and highlight thoughtful contributions</li>
              <li>Guide discussions without dominating them</li>
              <li>Provide timely responses to keep momentum</li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  `
})
export class DiscussionsComponent implements OnInit {
  darkMode = false;

  discussions: Discussion[] = [
    {
      id: 'd1',
      title: 'Challenges with Asynchronous JavaScript',
      course: 'Advanced JavaScript Concepts',
      creator: 'Sarah Wilson',
      creatorAvatar: 'assets/avatars/student3.jpg',
      date: '2 days ago',
      replies: 8,
      lastActivity: 'Just now',
      tags: ['JavaScript', 'Async']
    },
    {
      id: 'd2',
      title: 'Best practices for responsive design',
      course: 'Web Development Fundamentals',
      creator: 'Alex Johnson',
      creatorAvatar: 'assets/avatars/student1.jpg',
      date: '3 days ago',
      replies: 12,
      lastActivity: '3 hours ago',
      tags: ['CSS', 'Responsive']
    },
    {
      id: 'd3',
      title: 'React vs Angular debate',
      course: 'Full Stack Web Development',
      creator: 'David Kim',
      creatorAvatar: 'assets/avatars/student4.jpg',
      date: 'Last week',
      replies: 24,
      lastActivity: 'Yesterday',
      tags: ['React', 'Angular', 'Frameworks']
    },
    {
      id: 'd4',
      title: 'Database normalization help',
      course: 'Database Design',
      creator: 'Maria Garcia',
      creatorAvatar: 'assets/avatars/student2.jpg',
      date: '4 days ago',
      replies: 6,
      lastActivity: '2 days ago',
      tags: ['Databases', 'SQL']
    }
  ];

  messages: Message[] = [
    {
      id: 'm1',
      user: 'Jason Miller',
      avatar: 'assets/avatars/teacher1.jpg',
      content: 'Great question! Promises and async/await are different ways to handle asynchronous operations in JavaScript. Promises provide a .then() method for chaining, while async/await offers more readable, synchronous-looking code. For your issue with execution order, make sure you\'re properly awaiting all async operations.',
      timestamp: '1 day ago',
      replies: [
        {
          id: 'r1',
          user: 'Sarah Wilson',
          avatar: 'assets/avatars/student3.jpg',
          content: 'Thank you! That makes more sense now. I think I was missing some await keywords in my loop.',
          timestamp: '1 day ago'
        }
      ]
    },
    {
      id: 'm2',
      user: 'Michael Brown',
      avatar: 'assets/avatars/student5.jpg',
      content: 'I had a similar issue last week. The problem was that I was using Promise.all incorrectly. You need to pass an array of promises to it, not an array of async functions. Here\'s an example code snippet that might help...',
      timestamp: '12 hours ago'
    },
    {
      id: 'm3',
      user: 'Emily Chen',
      avatar: 'assets/avatars/student6.jpg',
      content: 'For debugging async issues, I find it helpful to use console.log statements with distinct labels before and after each async operation. This helps track the actual execution flow.',
      timestamp: '4 hours ago',
      replies: [
        {
          id: 'r2',
          user: 'Jason Miller',
          avatar: 'assets/avatars/teacher1.jpg',
          content: 'Great tip, Emily! I also recommend using the async/await debugger feature in Chrome DevTools.',
          timestamp: '3 hours ago'
        }
      ]
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
