import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DashboardComponent } from '../src/app/dashboard/dashboard.component';
import { Router } from '@angular/router';
import { By } from '@angular/platform-browser';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;
  let router: Router;

  beforeEach(async () => {
    const routerSpy = { navigate: jest.fn() };

    await TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [
        { provide: Router, useValue: routerSpy }
      ],
      schemas: [NO_ERRORS_SCHEMA] // For ignoring PrimeNG component warnings
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with default values', () => {
    expect(component.darkMode).toBeFalsy();
    expect(component.stats).toBeDefined();
    expect(component.stats.activeCourses).toEqual(5);
    expect(component.stats.totalStudents).toEqual(127);
    expect(component.stats.pendingAssignments).toEqual(23);
    expect(component.stats.averageRating).toEqual(4.8);
    expect(component.upcomingSessions.length).toEqual(4);
  });


  it('should navigate to specified route', () => {
    component.navigateTo('/courses');
    expect(router.navigate).toHaveBeenCalledWith(['/courses']);
  });

  it('should start a session and navigate to classroom', () => {
    jest.spyOn(console, 'log').mockImplementation(() => { });
    component.startSession('session1');
    expect(console.log).toHaveBeenCalledWith('Starting session session1');
    expect(router.navigate).toHaveBeenCalledWith(['/classroom/session1']);
  });

  it('should display teacher stats correctly', () => {
    const statElements = fixture.debugElement.queryAll(By.css('.text-5xl.font-bold'));
    expect(statElements.length).toEqual(4);
    expect(statElements[0].nativeElement.textContent.trim()).toEqual('5');
    expect(statElements[1].nativeElement.textContent.trim()).toEqual('127');
    expect(statElements[2].nativeElement.textContent.trim()).toEqual('23');
    expect(statElements[3].nativeElement.textContent.trim()).toEqual('4.8');
  });

  it('should have correct carousel responsive options', () => {
    expect(component.carouselResponsiveOptions.length).toEqual(3);
    expect(component.carouselResponsiveOptions[0].breakpoint).toEqual('1024px');
    expect(component.carouselResponsiveOptions[1].breakpoint).toEqual('768px');
    expect(component.carouselResponsiveOptions[2].breakpoint).toEqual('560px');
  });

  it('should have upcoming sessions with correct structure', () => {
    const firstSession = component.upcomingSessions[0];
    expect(firstSession.id).toBeDefined();
    expect(firstSession.date).toBeDefined();
    expect(firstSession.title).toBeDefined();
    expect(firstSession.description).toBeDefined();
    expect(firstSession.time).toBeDefined();
    expect(firstSession.students).toBeDefined();
    expect(typeof firstSession.students).toBe('number');
  });

  it('should display different styles based on dark mode', () => {
    // Test with dark mode off
    component.darkMode = false;
    fixture.detectChanges();

    let dashboardTitle = fixture.debugElement.query(By.css('h2.text-2xl.font-bold'));
    expect(dashboardTitle.nativeElement.classList).toContain('text-gray-800');
    expect(dashboardTitle.nativeElement.classList).not.toContain('text-white');

    // Change to dark mode
    component.darkMode = true;
    fixture.detectChanges();

    dashboardTitle = fixture.debugElement.query(By.css('h2.text-2xl.font-bold'));
    expect(dashboardTitle.nativeElement.classList).toContain('text-white');
    expect(dashboardTitle.nativeElement.classList).not.toContain('text-gray-800');
  });

  it('should have the correct number of upcoming sessions', () => {
    expect(component.upcomingSessions.length).toBe(4);

    // Verify each session has the expected properties
    component.upcomingSessions.forEach(session => {
      expect(session.id).toBeTruthy();
      expect(session.date).toBeTruthy();
      expect(session.title).toBeTruthy();
      expect(session.description).toBeTruthy();
      expect(session.time).toBeTruthy();
      expect(typeof session.students).toBe('number');
    });
  });

  it('should have correct data for first upcoming session', () => {
    const firstSession = component.upcomingSessions[0];
    expect(firstSession.id).toEqual('session1');
    expect(firstSession.date).toEqual('23');
    expect(firstSession.title).toEqual('Web Development Fundamentals');
    expect(firstSession.description).toEqual('Introduction to HTML5 and CSS3 core concepts');
    expect(firstSession.time).toEqual('Today at 2:00 PM');
    expect(firstSession.students).toEqual(24);
  });

  it('should render the feature grid with 3 sections', () => {
    const featureSections = fixture.debugElement.queryAll(By.css('.max-w-7xl.w-full.mx-auto.grid > div'));
    expect(featureSections.length).toBe(3);

    // Check titles of the sections
    const sectionTitles = fixture.debugElement.queryAll(By.css('.max-w-7xl.w-full.mx-auto.grid h3.text-xl'));
    expect(sectionTitles.length).toBe(3);
    expect(sectionTitles[0].nativeElement.textContent.trim()).toEqual('Course Management');
    expect(sectionTitles[1].nativeElement.textContent.trim()).toEqual('Student Assessment');
    expect(sectionTitles[2].nativeElement.textContent.trim()).toEqual('Classroom Discussions');
  });

  it('should have buttons with correct navigation routes', () => {
    jest.spyOn(component, 'navigateTo');

    const buttons = fixture.debugElement.queryAll(By.css('p-button'));

    // Find the "Manage Courses" button by its parent container
    const courseManagementSection = fixture.debugElement.queryAll(By.css('.max-w-7xl.w-full.mx-auto.grid > div'))[0];
    const manageCourseBtn = courseManagementSection.query(By.css('p-button'));
    manageCourseBtn.triggerEventHandler('onClick', null);

    expect(component.navigateTo).toHaveBeenCalledWith('/courses');
  });

  it('should render resource section with correct title', () => {
    const resourceSection = fixture.debugElement.query(By.css('.text-center.w-full.max-w-3xl.mx-auto'));
    const title = resourceSection.query(By.css('h3'));

    expect(title.nativeElement.textContent.trim()).toEqual('Teaching Resources');
  });

  it('should properly handle session navigation for all session types', () => {
    // Test multiple sessions
    component.upcomingSessions.forEach(session => {
      jest.clearAllMocks();
      jest.spyOn(console, 'log');

      component.startSession(session.id);

      expect(console.log).toHaveBeenCalledWith(`Starting session ${session.id}`);
      expect(router.navigate).toHaveBeenCalledWith([`/classroom/${session.id}`]);
    });
  });
});
