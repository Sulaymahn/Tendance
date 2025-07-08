import { Routes } from '@angular/router';
import { FaceRegistrationComponent } from './face-registration/face-registration.component';

export const routes: Routes = [
    {
        path: '',
        component: FaceRegistrationComponent
    },
    {
        path: '**',
        redirectTo: '/'
    }
];
