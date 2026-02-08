import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

@Component({
    selector: 'app-text-input',
    templateUrl: './text-input.component.html',
    styleUrls: ['./text-input.component.scss'],
    standalone: false
})
export class TextInputComponent implements ControlValueAccessor {
  @Input() type = 'text'
  @Input() label = ''

  constructor(@Self() public controlDir: NgControl){
    this.controlDir.valueAccessor = this
  }
  //self so angular don't reuse upon injection , we want it to be uniq for every input
  writeValue(obj: any): void {
    
  }
  registerOnChange(fn: any): void {
    
  }
  registerOnTouched(fn: any): void {
    
  }

  get control(): FormControl {
    return this.controlDir.control as FormControl
  }
  

}
