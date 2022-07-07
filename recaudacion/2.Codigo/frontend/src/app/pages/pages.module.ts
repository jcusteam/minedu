import { ListUeComponent } from './../theme/components/list-ue/list-ue.component';
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { PagesComponent } from "./pages.component";
import { BreadcrumbComponent } from "../theme/components/breadcrumb/breadcrumb.component";
import { FlagsMenuComponent } from "../theme/components/flags-menu/flags-menu.component";
import { FullScreenComponent } from "../theme/components/fullscreen/fullscreen.component";
import { HorizontalMenuComponent } from "../theme/components/menu/horizontal-menu/horizontal-menu.component";
import { VerticalMenuComponent } from "../theme/components/menu/vertical-menu/vertical-menu.component";
import { MessagesComponent } from "../theme/components/messages/messages.component";
import { SidenavComponent } from "../theme/components/sidenav/sidenav.component";
import { UserMenuComponent } from "../theme/components/user-menu/user-menu.component";
import { IndexComponent } from "./index/index.component";
import { PageRoutingModule } from "./pages.routing";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { PerfectScrollbarModule } from "ngx-perfect-scrollbar";
import { SharedModule } from "../shared/shared.module";
import { PipesModule } from "../theme/pipes/pipes.module";
import { MineduSharedModule } from "src/@minedu/minedu.shared.module";
import { MatPaginatorIntl } from "@angular/material/paginator";
import { CustomMatPaginatorIntl } from "src/@minedu/intl/custom-mat-paginator-intl";
import { ApplicationsComponent } from '../theme/components/applications/applications.component';

@NgModule({
  imports: [
    CommonModule,
    PageRoutingModule,
    FormsModule,
    MineduSharedModule,
    ReactiveFormsModule,
    PerfectScrollbarModule,
    SharedModule,
    PipesModule,
  ],
  declarations: [
    PagesComponent,
    IndexComponent,
    SidenavComponent,
    VerticalMenuComponent,
    HorizontalMenuComponent,
    BreadcrumbComponent,
    FlagsMenuComponent,
    FullScreenComponent,
    ApplicationsComponent,
    MessagesComponent,
    UserMenuComponent,
    ListUeComponent
  ],
  entryComponents: [VerticalMenuComponent],
  providers: [
    { provide: MatPaginatorIntl, useClass: CustomMatPaginatorIntl },
  ]
})
export class PagesModule { }
