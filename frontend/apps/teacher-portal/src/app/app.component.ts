import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { PrimeNG } from 'primeng/config';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'vanguard-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ButtonModule
  ],
  template: `
    <div class="min-h-screen flex flex-col" [ngClass]="{'bg-gray-100': !darkMode, 'bg-gray-800 text-white': darkMode}">
      <!-- Header Section -->
      <header class="w-full py-4 px-6" [ngClass]="{'bg-white shadow-sm': !darkMode, 'bg-gray-900 shadow-md': darkMode}">
        <div class="max-w-7xl mx-auto flex justify-between items-center">
          <div class="flex items-center">
            <img src="assets/vanguard-logo.svg" alt="Vanguard Logo" class="h-8 w-auto mr-3">
            <h1 class="text-2xl font-bold" [ngClass]="{'text-gray-900': !darkMode, 'text-white': darkMode}">
              Vanguard Teacher Portal
            </h1>
          </div>
          <div class="flex items-center gap-4">
            <button (click)="toggleDarkMode()" class="p-2 rounded-full"
              [ngClass]="{'bg-gray-200 hover:bg-gray-300': !darkMode, 'bg-gray-700 hover:bg-gray-600': darkMode}">
              <span *ngIf="darkMode">☀️</span>
              <span *ngIf="!darkMode">🌙</span>
            </button>
            <p-button label="Dashboard" (onClick)="navigateTo('/dashboard')"
              styleClass="p-button-text"></p-button>
            <p-button label="Courses" (onClick)="navigateTo('/courses')"
              styleClass="p-button-text"></p-button>
            <p-button label="Assessments" (onClick)="navigateTo('/assessments')"
              styleClass="p-button-text"></p-button>
          </div>
        </div>
      </header>

      <!-- Main Content Area -->
      <main class="flex-1">
        <router-outlet></router-outlet>
      </main>

      <!-- Footer -->
      <footer class="w-full py-6 px-4"
        [ngClass]="{'bg-white border-t border-gray-200': !darkMode, 'bg-gray-900 border-t border-gray-700': darkMode}">
        <div class="max-w-7xl mx-auto flex flex-col md:flex-row justify-between items-center">
          <p [ngClass]="{'text-gray-600': !darkMode, 'text-gray-300': darkMode}">
            © 2025 Vanguard Skill Development Platform
          </p>
          <div class="flex gap-4 mt-4 md:mt-0">
            <a href="#" [ngClass]="{'text-gray-600 hover:text-gray-900': !darkMode, 'text-gray-300 hover:text-white': darkMode}">
              Instructor Support
            </a>
            <a href="#" [ngClass]="{'text-gray-600 hover:text-gray-900': !darkMode, 'text-gray-300 hover:text-white': darkMode}">
              Privacy Policy
            </a>
            <a href="#" [ngClass]="{'text-gray-600 hover:text-gray-900': !darkMode, 'text-gray-300 hover:text-white': darkMode}">
              Terms of Service
            </a>
          </div>
        </div>
      </footer>
    </div>
  `
})
export class AppComponent implements OnInit {
  darkMode = false;

  constructor(
    private router: Router,
    private primeng: PrimeNG
  ) { }

  ngOnInit() {
    // Enable ripple effect
    this.primeng.ripple.set(true);

    // Check localStorage for dark mode preference
    const storedDarkMode = localStorage.getItem('darkMode');
    if (storedDarkMode) {
      this.darkMode = storedDarkMode === 'true';
      if (this.darkMode) {
        document.querySelector('html')?.classList.add('dark-mode');
      }
    }
  }

  toggleDarkMode() {
    this.darkMode = !this.darkMode;

    // Update HTML class for global styling
    const element = document.querySelector('html');
    element?.classList.toggle('dark-mode');

    // Save preference to localStorage
    localStorage.setItem('darkMode', this.darkMode.toString());
  }

  navigateTo(route: string) {
    this.router.navigate([route]);
  }
}
