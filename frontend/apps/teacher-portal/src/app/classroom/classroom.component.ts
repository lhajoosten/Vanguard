import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'vanguard-classroom',
  standalone: true,
  imports: [CommonModule, ButtonModule],
  template: `
    <div class="p-8 max-w-6xl mx-auto">
      <h1 class="text-3xl font-bold mb-6" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">
        Virtual Classroom: {{sessionId}}
      </h1>

      <div class="bg-blue-100 text-blue-800 p-4 rounded mb-8">
        This is a placeholder for the virtual classroom interface.
        In a complete implementation, this would include video conferencing, whiteboard, chat, etc.
      </div>

      <div class="aspect-video w-full bg-gray-900 mb-8 rounded-lg flex items-center justify-center">
        <p class="text-white text-xl">Video Conference Area</p>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div class="border p-4 rounded" [ngClass]="{'bg-white': !darkMode, 'bg-gray-800': darkMode}">
          <h3 class="font-bold mb-2" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">Participants (24)</h3>
          <div class="h-40 overflow-y-auto">
            <p [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">Student list would appear here</p>
          </div>
        </div>

        <div class="border p-4 rounded md:col-span-2" [ngClass]="{'bg-white': !darkMode, 'bg-gray-800': darkMode}">
          <h3 class="font-bold mb-2" [ngClass]="{'text-gray-800': !darkMode, 'text-white': darkMode}">Chat</h3>
          <div class="h-40 overflow-y-auto">
            <p [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">Chat messages would appear here</p>
          </div>
        </div>
      </div>

      <div class="flex justify-end">
        <p-button label="End Session" severity="danger"></p-button>
      </div>
    </div>
  `
})
export class ClassroomComponent implements OnInit {
  darkMode = false;
  sessionId: string = '';

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    // Get the session ID from the route parameter
    this.route.paramMap.subscribe(params => {
      this.sessionId = params.get('id') || '';
    });

    // Check localStorage for dark mode preference
    const storedDarkMode = localStorage.getItem('darkMode');
    if (storedDarkMode) {
      this.darkMode = storedDarkMode === 'true';
    }
  }
}
