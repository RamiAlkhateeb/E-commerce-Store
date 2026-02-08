import { Component, OnInit } from '@angular/core';
import { OrdersService } from '../orders.service';
import { Order } from 'src/app/shared/models/order';

@Component({
    selector: 'app-orders',
    templateUrl: './orders.component.html',
    styleUrls: ['./orders.component.scss'],
    standalone: false
})
export class OrdersComponent implements OnInit{

  orders : Order[]= []

  constructor(private orderService: OrdersService){}

  ngOnInit(): void {
    
    this.orderService.getOrders().subscribe({
      next: orders => this.orders = orders
    })
  }

}
