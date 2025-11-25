import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

    if (!auth.token) { 
      auth.logout()
      router.navigateByUrl('/login');
      return false; 
    }

  if (auth.role() != "Student") {
    auth.logout()
    router.navigateByUrl('/login');
    return false;
  }
  return true;
};