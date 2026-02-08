import { Injectable, inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateChildFn, CanActivateFn, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { catchError, map, Observable, of } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';

//@Injectable({
//  providedIn: 'root'
//})
// export class AuthGuard implements CanActivate {

//   constructor(private accountService : AccountService ,
//     private router : Router){}
//   canActivate(
//     route: ActivatedRouteSnapshot,
//     state: RouterStateSnapshot): Observable<boolean >  {
//     return this.accountService.currentUser$.pipe(
//       map(auth => {
//         if (auth) return true
//         else{
//           this.router.navigate(['/account/login'] , {queryParams: {returnUrl : state.url}})
//           return false
//         }
//       })
//     );
//   }
  
// }

export const canActivate: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const authService = inject(AccountService);
  const router = inject(Router);

  return authService.currentUser$.pipe(
    map(() => true),
    catchError(() => {
      router.navigate(['/account/login']);
      return of(false);
    })
  );
};

export const canActivateChild: CanActivateChildFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => canActivate(route, state);