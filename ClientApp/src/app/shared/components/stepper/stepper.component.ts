import { CdkStepper } from '@angular/cdk/stepper';
import { Component, Input, OnInit } from '@angular/core';

@Component({
    selector: 'app-stepper',
    templateUrl: './stepper.component.html',
    styleUrls: ['./stepper.component.scss'],
    providers: [{ provide: CdkStepper, useExisting: StepperComponent }],
    standalone: false
})
export class StepperComponent extends CdkStepper implements OnInit{
  
  @Input() linearModeSelected = true; // user can't proceed to
  // step 3 until he completes step 2

  ngOnInit(): void {
    this.linear = this.linearModeSelected
  }

  onClick(index : number){
    this.selectedIndex = index
  }
}
