import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Product } from '../shared/models/product';
import { ShopParams } from '../shared/models/shopParams';
import { ShopService } from './shop.service';


@Component({
    selector: 'app-shop',
    templateUrl: './shop.component.html',
    styleUrls: ['./shop.component.scss'],
    standalone: false
})
export class ShopComponent implements OnInit {
  @ViewChild('search') searchTerm?: ElementRef
  products: Product[] = [];
  categories: string[] = [];
  brands: string[] = [];
  shopParams: ShopParams
  totalCount = 0

  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price low to high', value: 'priceAsc' },
    { name: 'Price High to low', value: 'priceDesc' }

  ]

  constructor(private shopService: ShopService) {
    this.shopParams = shopService.getShopParams()
  }
  ngOnInit(): void {
    this.getProducts();
    this.getCategories();
    this.getBrands();
  }

  getProducts() {
    this.shopService.getProducts().subscribe({
      next: response => {
        this.products = response.data

        this.totalCount = response.count
      },
      error: error => console.log(error),
    })
  }

  getCategories() {
    this.shopService.getTypes().subscribe({
      next: response => this.categories = ['All', ...response],
      error: error => console.log(error),
    })
  }

  getBrands() {
    this.shopService.getBrands().subscribe({
      next: response => this.brands = ['All', ...response],
      error: error => console.log(error),
    })
  }

  onTypeSelected(category: string) {
    const params = this.shopService.getShopParams()
    params.category = category
    params.pageNumber = 1
    this.shopService.setShopParams(params)
    this.shopParams = params
    this.getProducts()
  }

  onBrandSelected(brand: string) {
    const params = this.shopService.getShopParams()
    params.brand = brand
    params.pageNumber = 1
    this.shopService.setShopParams(params)
    this.shopParams = params
    this.getProducts()
  }

  onSortSelected(event: any) {
    const params = this.shopService.getShopParams()
    params.sort = event.target.value
    this.shopService.setShopParams(params)
    this.shopParams = params
    this.getProducts()
  }

  onPageChanged(event: any) {
    const params = this.shopService.getShopParams()
    if (params.pageNumber !== event) {
      params.pageNumber = event
      this.shopService.setShopParams(params)
      this.shopParams = params
      this.getProducts()
    }
  }

  onSearch() {
    const params = this.shopService.getShopParams()
    params.search = this.searchTerm?.nativeElement.value
    params.pageNumber = 1
    this.shopService.setShopParams(params)
    this.shopParams = params 
    this.getProducts()
  }

  onReset() {
    if (this.searchTerm) this.searchTerm.nativeElement.value = ''
    this.shopParams= new ShopParams()
    this.shopService.setShopParams(this.shopParams)
    this.getProducts()
  }

}
