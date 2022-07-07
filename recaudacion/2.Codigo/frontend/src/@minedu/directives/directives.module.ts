import { NgModule } from '@angular/core';
import { BlockCopyPasteDirective } from './block-copy-paste/block-copy-paste.directive';
import { DigitalNumberDecimalDirective } from './digital-number-decimal/digital-number-decimal.directive';
import { OnlyNumberDirective } from './only-number/only-number.directive';
import { StopPropagationDirective } from './stop-propagation/stop-propagation.directive';

@NgModule({
  declarations: [
    OnlyNumberDirective,
    BlockCopyPasteDirective,
    DigitalNumberDecimalDirective,
    StopPropagationDirective
  ],
  exports: [
    OnlyNumberDirective,
    BlockCopyPasteDirective,
    DigitalNumberDecimalDirective,
    StopPropagationDirective
  ]
})
export class DirectivesModule { }
