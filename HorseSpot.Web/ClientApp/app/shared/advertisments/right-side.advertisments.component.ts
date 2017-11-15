import { Component, OnInit } from '@angular/core';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

declare var window: any;

@Component({
  selector: 'right-side-advertisments',
  templateUrl: './right-side.advertisments.component.html'
})

export class RightSideAdvertismentsComponent implements OnInit {

  constructor(@Inject(PLATFORM_ID) private platformId: Object) { }

  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      window.FB.XFBML.parse();
    }
  }
}
