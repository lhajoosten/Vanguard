import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { PrimeNG } from 'primeng/config';

@Component({
  selector: 'vanguard-root',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="min-h-screen flex flex-col" [ngClass]="{'bg-gray-100': !darkMode, 'bg-gray-800 text-white': darkMode}">
      <!-- Header Section -->
      <header class="w-full py-4 px-6" [ngClass]="{'bg-white shadow-sm': !darkMode, 'bg-gray-900 shadow-md': darkMode}">
        <div class="max-w-7xl mx-auto flex justify-between items-center">
          <h1 class="text-2xl font-bold" [ngClass]="{'text-blue-600': !darkMode, 'text-blue-300': darkMode}">
            Vanguard Student Portal
          </h1>
          <div class="flex items-center gap-4">
            <button (click)="toggleDarkMode()" class="p-2 rounded-full"
              [ngClass]="{'bg-gray-200 hover:bg-gray-300': !darkMode, 'bg-gray-700 hover:bg-gray-600': darkMode}">
              <span *ngIf="darkMode">☀️</span>
              <span *ngIf="!darkMode">🌙</span>
            </button>
            <button (click)="navigateTo('/dashboard')" class="px-4 py-2 rounded"
              [ngClass]="{'bg-blue-100 text-blue-700': !darkMode, 'bg-blue-800 text-white': darkMode}">
              Dashboard
            </button>
            <button (click)="navigateTo('/recommendations')" class="px-4 py-2 rounded"
              [ngClass]="{'bg-blue-100 text-blue-700': !darkMode, 'bg-blue-800 text-white': darkMode}">
              Recommendations
            </button>
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
    if (this.darkMode) {
      document.querySelector('html')?.classList.add('dark-mode');
    } else {
      document.querySelector('html')?.classList.remove('dark-mode');
    }

    // Save preference to localStorage
    localStorage.setItem('darkMode', this.darkMode.toString());
  }

  navigateTo(route: string) {
    this.router.navigate([route]);
  }
}
