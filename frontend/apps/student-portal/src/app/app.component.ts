import { Component, OnInit } from '@angular/core';
import { PrimeNG } from 'primeng/config';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CarouselModule } from 'primeng/carousel';
import { Router } from '@angular/router';

interface Activity {
  avatar: string;
  title: string;
  description: string;
  time: string;
}

@Component({
  selector: 'app-root',
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    CarouselModule
  ],
  templateUrl: './app.component.html',
  standalone: true,
})
export class AppComponent implements OnInit {
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

  constructor(
    private primeng: PrimeNG,
    private router: Router
  ) { }

  ngOnInit() {
    this.primeng.ripple.set(true);
    // Check for saved theme preference
    const savedDarkMode = localStorage.getItem('darkMode');
    if (savedDarkMode === 'true') {
      this.darkMode = true;
      document.querySelector('html')?.classList.add('dark-mode');
    }
  }

  toggleDarkMode() {
    this.darkMode = !this.darkMode;
    const element = document.querySelector('html');
    element?.classList.toggle('dark-mode');

    // Save preference to localStorage
    localStorage.setItem('darkMode', this.darkMode.toString());
  }

  enterPortal() {
    // Navigate to the main dashboard
    this.navigateTo('/dashboard');
  }

  navigateTo(route: string) {
    // Navigate to the specified route
    this.router.navigate([route]);
    console.log(`Navigating to ${route}...`);
  }
}
