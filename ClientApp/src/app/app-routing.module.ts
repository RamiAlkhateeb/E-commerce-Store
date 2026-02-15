import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountModule } from './account/account.module';
import { BasketModule } from './basket/basket.module';
import { CheckoutModule } from './checkout/checkout.module';
import { canActivate } from './core/guards/auth.guard';
import { NotFoundComponent } from './core/not-found/not-found.component';
import { ServerErrorComponent } from './core/server-error/server-error.component';
import { ProfileComponent } from './profile/profile.component';
import { ShopModule } from './shop/shop.module';
import { OrderModule } from './order/order.module';
import { ShopComponent } from './shop/shop.component';

const routes: Routes = [
  { path: 'shop', loadChildren: () => import('./shop/shop.module').then(m => ShopModule) },
  { path: 'basket', loadChildren: () => import('./basket/basket.module').then(m => BasketModule) },
  { path: 'account', loadChildren: () => import('./account/account.module').then(m => AccountModule) },
  { path: 'orders', 
    canActivate: [canActivate],
    loadChildren: () => import('./order/order.module').then(m => OrderModule) 
  },
  {
    path: 'checkout',
    canActivate : [canActivate],
    loadChildren: () => import('./checkout/checkout.module').then(m => CheckoutModule)
  },
  { path: '', component: ShopComponent, pathMatch: 'full' },
  { path: 'not-found', component: NotFoundComponent },
  { path: 'server-error', component: ServerErrorComponent },
  { path: '**', redirectTo: '', pathMatch: 'full' }

  //{path: 'account' , c}

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
