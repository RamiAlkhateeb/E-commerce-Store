import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs';
import { BasketService } from 'src/app/basket/basket.service';
import { Product } from 'src/app/shared/models/product';
import { ShopService } from '../shop.service';

@Component({
    selector: 'app-product-details',
    templateUrl: './product-details.component.html',
    styleUrls: ['./product-details.component.scss'],
    standalone: false
})
export class ProductDetailsComponent implements OnInit {
  product?: Product
  images: string[] = []
  quantity : number = 1
  quantityInBasket : number = 0

  constructor(private shopService: ShopService, 
    private activatedRoute: ActivatedRoute,
    private basketService: BasketService) {

  }
  ngOnInit(): void {
    
    this.loadProduct()
  }

  loadProduct() {
    const id = this.activatedRoute.snapshot.paramMap.get('id')
    if (id)
      this.shopService.getProduct(+id).subscribe({
        next: product => {
          this.product = product
          if (this.product.productImages.length == 0)
            this.images[0] = this.product.thumbnail
          else
            this.images = this.product.productImages
            // here will use take 1 to say that we will take 
            // 1 thing and then unsubscribe to the observable 
            this.basketService.basketSource$.pipe(take(1)).subscribe({
              next: basket => {
                const item = basket?.items.find(x => x.id === +id)
                if (item){
                  this.quantity = item.quantity
                  this.quantityInBasket = item.quantity
                }
              }
            })
        }

      })
  }

  incrementQuantity(){
    this.quantity++
  }

  decrementQuantity(){
    if(this.quantity > 1) this.quantity--
  }

  updateBasket(){
    if (this.product){
      if(this.quantity > this.quantityInBasket){
        const itemsToAdd = this.quantity - this.quantityInBasket
        this.quantityInBasket += itemsToAdd
        this.basketService.addItemToBasket(this.product , itemsToAdd)
      }else{
        const itemsToRemove = this.quantityInBasket - this.quantity
        this.quantityInBasket -= itemsToRemove
        this.basketService.removeItemFromBasket(this.product.id, itemsToRemove)
      }
    }
  }

  get buttonText(){
    return this.quantityInBasket === 0 ? 'Add to basket' : 'Update basket'
  }

}
