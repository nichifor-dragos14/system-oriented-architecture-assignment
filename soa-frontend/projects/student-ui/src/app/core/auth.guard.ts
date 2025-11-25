import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  console.log("tries", auth.token)
  if (!auth.token) { 
    router.navigateByUrl('/login');
    return false; 
  }

  return true;
};