import { Component, OnInit } from '@angular/core';

declare var window: any;

@Component({
  selector: 'right-side-advertisments',
  templateUrl: './right-side.advertisments.component.html'
})

export class RightSideAdvertismentsComponent implements OnInit {

  ngOnInit() {
    window.FB.XFBML.parse();
  }
}
