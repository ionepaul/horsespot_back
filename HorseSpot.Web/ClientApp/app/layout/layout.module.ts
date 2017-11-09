import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';
import { NavbarComponent } from './navbar/navbar.component';
import { NavbarUserPartComponent} from './navbar/navbar-user-part/navbar.user.part.component';
import { FooterComponent } from './footer/footer.component';

@NgModule({
    imports: [ 
        SharedModule,
        RouterModule
  ],
  declarations: [ NavbarComponent, NavbarUserPartComponent, FooterComponent ],
  exports: [ NavbarComponent, FooterComponent ]
})

export class LayoutModule { }