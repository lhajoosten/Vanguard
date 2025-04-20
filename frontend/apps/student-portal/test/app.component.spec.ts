import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { provideRouter, Router } from '@angular/router';
import { AppComponent } from '../src/app/app.component';
import { PrimeNG } from 'primeng/config';

// Create a simple localStorage mock
const localStorageMock = (() => {
  let store: Record<string, string> = {};
  return {
    getItem: jest.fn((key: string) => store[key] || null),
    setItem: jest.fn((key: string, value: string) => { store[key] = value; }),
    clear: jest.fn(() => { store = {}; })
  };
})();

const primengMock = {
  ripple: {
    set: jest.fn()
  }
};

// Replace window.localStorage with our mock
Object.defineProperty(window, 'localStorage', { value: localStorageMock });

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [
        provideRouter([]),
        {
          provide: PrimeNG,
          useValue: primengMock
        }
      ],
      schemas: [NO_ERRORS_SCHEMA] // Avoid errors with unknown elements
    }).compileComponents();
  });

  beforeEach(() => {
    // Clear mocks before each test
    localStorageMock.clear();
    jest.clearAllMocks();

    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have dark mode disabled by default', () => {
    expect(component.darkMode).toBe(false);
  });

  it('should toggle dark mode when method is called', () => {
    const initialValue = component.darkMode;

    component.toggleDarkMode();

    expect(component.darkMode).toBe(!initialValue);
    expect(localStorageMock.setItem).toHaveBeenCalledWith('darkMode', (!initialValue).toString());
  });

  it('should read dark mode from localStorage on init', () => {
    // Set localStorage before component initialization
    localStorageMock.getItem.mockReturnValueOnce('true');

    // Recreate component to trigger ngOnInit
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    component.ngOnInit();

    expect(component.darkMode).toBe(true);
  });

  it('should navigate when navigateTo is called', () => {
    const routerSpy = jest.spyOn(TestBed.inject(Router), 'navigate');
    component.navigateTo('/test-route');
    expect(routerSpy).toHaveBeenCalledWith(['/test-route']);
  });
});
