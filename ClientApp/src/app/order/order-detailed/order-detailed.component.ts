import { Component, OnInit } from '@angular/core';
import { OrdersService } from '../orders.service';
import { ActivatedRoute } from '@angular/router';
import { Order } from 'src/app/shared/models/order';

@Component({
    selector: 'app-order-detailed',
    templateUrl: './order-detailed.component.html',
    styleUrls: ['./order-detailed.component.scss'],
    standalone: false
})
export class OrderDetailedComponent implements OnInit{

  order?: Order

  constructor(private orderService : OrdersService,
    private activatedRoute : ActivatedRoute){}

  ngOnInit(): void {
    this.loadOrder()
  }

  loadOrder(){
    const id = this.activatedRoute.snapshot.paramMap.get('id')
    if(id)
      this.orderService.getOrderById(+id).subscribe({
        next : order => {
          this.order = order
        }
      })
  }



}
