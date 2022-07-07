import { NgModule } from '@angular/core';
import { MineduNumberDirective } from './minedu-number.directive';
import { MineduBlockCopyPasteDirective } from './minedu-block-copy-paste.directive';

@NgModule({
  imports: [],
  declarations: [
    MineduNumberDirective,
    MineduBlockCopyPasteDirective
  ],
  exports: [
    MineduNumberDirective,
    MineduBlockCopyPasteDirective
  ]
})
export class DirectivesModule { }
