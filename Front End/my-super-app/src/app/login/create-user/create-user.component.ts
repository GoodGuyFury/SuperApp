import { Component, EventEmitter, NgZone, OnInit, Output } from '@angular/core';
import { googleClientId } from '../../enviorment';
import {Location} from '@angular/common';

declare var google :any;

@Component({
  selector: 'app-create-user',
  templateUrl: './create-user.component.html',
  styleUrls: ['./create-user.component.scss'],
  standalone : true,
  imports:[]
})
export class CreateUserComponent implements OnInit {

  @Output() createUserResponse = new EventEmitter<boolean>();

  constructor(private ngZone: NgZone, private location :Location) { }

  ngOnInit() {
    google.accounts.id.initialize({
      client_id: googleClientId,
      callback:(resp:any)=>{
        this.ngZone.run(() => {
        });
      }
    });

    google.accounts.id.prompt();


    google.accounts.id.renderButton(document.getElementById("google-btn"),{
      type: "standard",
      theme: "filled_blue",
      size: "large",
      shape: "rectangle",
      width: 100,
      logo_alignment: "center",
      height: "auto",
    })
  }

  goBackToSignIn(){
    this.createUserResponse.emit(true);
  }
}
