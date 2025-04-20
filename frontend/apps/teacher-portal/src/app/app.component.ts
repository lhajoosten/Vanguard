import { Component, OnInit } from '@angular/core';
import { PrimeNG } from 'primeng/config';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
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

  enterDashboard() {
    // Navigate to the main dashboard
    this.navigateTo('/dashboard');
  }

  navigateTo(route: string) {
    // Navigate to the specified route
    this.router.navigate([route]);
    console.log(`Navigating to ${route}...`);
  }

  startSession(sessionId: string) {
    console.log(`Starting session ${sessionId}`);
    // Implementation for starting a virtual class session
    this.navigateTo(`/classroom/${sessionId}`);
  }
}
