import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AccountService } from '../account/account.service';
import { BasketService } from '../basket/basket.service';
import { NavigationExtras, Router } from '@angular/router';
import { Basket } from '../shared/models/basket';
import { ToastrService } from 'ngx-toastr';
import { CheckoutService } from './checkout.service';
import { OrderToCreate } from '../shared/models/order';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
  standalone: false
})
export class CheckoutComponent implements OnInit {

  loading = false


  constructor(private fb: FormBuilder,
    private checkoutService: CheckoutService,
    private accountService: AccountService,
    private basketService: BasketService,
    private toastr: ToastrService,
    private router: Router) { }

  ngOnInit(): void {
    //this.getAddressFormValues()
    //this.getDeliveryMethodValue()
  }

  checkoutForm = this.fb.group({
    addressForm: this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      street: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?\d{10,15}$/)]],
    })
  })

  // getAddressFormValues() {
  //   this.accountService.getUserAddress().subscribe({
  //     next: address => {
  //       address && this.checkoutForm.get('addressForm')?.patchValue(address)
  //     }
  //   })
  // }

  // getDeliveryMethodValue() {
  //   const basket = this.basketService.getCurrnetBasketValue()
  //   if (basket && basket.deliveryMethodId) {
  //     this.checkoutForm.get('deliveryForm')?.get('deliveryMethod')
  //       ?.patchValue(basket.deliveryMethodId.toString())
  //   }
  // }

  async submitOrder() {
    this.loading = true;
    const basket = this.basketService.getCurrnetBasketValue();

    if (!basket) {
      this.toastr.error('Cannot get basket');
      this.loading = false;
      return;
    }

    try {
      // 1. Call the API to create order (Backend will handle WhatsApp)
      const orderToCreate = this.getOrderToCreate(basket);
      const createdOrder = await this.checkoutService.createOrder(orderToCreate).toPromise();

      // 2. If successful, clear basket and redirect
      this.basketService.deleteBasket(basket);

      const navigationExtras: NavigationExtras = { state: createdOrder };
      this.router.navigate(['checkout/success'], navigationExtras);

    } catch (error: any) {
      console.log(error);
      this.toastr.error(error.message || 'An error occurred while placing the order');
    } finally {
      this.loading = false;
    }
  }

  private getOrderToCreate(basket: Basket): OrderToCreate {
    const formValues = this.checkoutForm.get('addressForm')?.value;
    
    if (!formValues) throw new Error('Address form is missing');
    return {
      basketId: basket.id,
      shipToAddress: {
        firstName: formValues.firstName || "",
        lastName: formValues.lastName || "",
        street: formValues.street || "",
        city: formValues.city || "",
        country: formValues.country || "",
        zipCode: formValues.phoneNumber || ""
      },
      deliveryMethodId: 1, // Dummy value
      //nameOnCard: 'Dummy Name' // Dummy value
    };
  }
}
