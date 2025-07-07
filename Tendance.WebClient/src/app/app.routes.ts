import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { SignupComponent } from './signup/signup.component';
import { TeachersComponent } from './teachers/teachers.component';
import { MainlayoutComponent } from './mainlayout/mainlayout.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CoursesComponent } from './courses/courses.component';
import { StudentsComponent } from './students/students.component';
import { ClassroomsComponent } from './classrooms/classrooms.component';
import { WebhooksComponent } from './webhooks/webhooks.component';
import { DevicesComponent } from './devices/devices.component';
import { AuthenticationService } from '../services/authentication/authentication.service';
import { inject } from '@angular/core';
import { RoomsComponent } from './rooms/rooms.component';

const canActivateApp: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    const auth = inject(AuthenticationService);
    if (auth.isAuthenticated()) {
        return true;
    }

    const router = inject(Router);
    return router.createUrlTree(['/login'], {
        queryParams: {
            returnUrl: state.url
        }
    });
};

const redirectIfAuthenticatedFn: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    const auth = inject(AuthenticationService);
    if (auth.isAuthenticated()) {
        const router = inject(Router);
        return router.createUrlTree(['decks']);
    }

    return true;
};

export const routes: Routes = [
    {
        path: 'login',
        canActivate: [redirectIfAuthenticatedFn],
        component: LoginComponent
    },
    {
        path: 'signup',
        canActivate: [redirectIfAuthenticatedFn],
        component: SignupComponent
    },
    {
        path: '',
        canActivate: [canActivateApp],
        component: MainlayoutComponent,
        children: [
            {
                path: 'dashboard',
                component: DashboardComponent
            },
            {
                path: 'teachers',
                component: TeachersComponent
            },
            {
                path: 'courses',
                component: CoursesComponent
            },
            {
                path: 'students',
                component: StudentsComponent
            },
            {
                path: 'classrooms',
                component: ClassroomsComponent
            },
            {
                path: 'webhooks',
                component: WebhooksComponent
            },
            {
                path: 'devices',
                component: DevicesComponent
            },
            {
                path: 'rooms',
                component: RoomsComponent
            },
            {
                path: '**',
                redirectTo: '/dashboard'
            },
        ]
    },
    {
        path: '**',
        redirectTo: '/login'
    }
];