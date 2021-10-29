import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  constructor(public accountService:AccountService) {
    console.log(accountService.currentUser$)
   }
  model:any={};
  IsLoggedIn:boolean=false;
  
  ngOnInit(): void {
    //this.getCurrentUser();
   // this.currentUser$=this.accountService.currentUser$;
  }

  login(){
    this.accountService.login(this.model).subscribe(responce=>{
     this.IsLoggedIn=true;
      console.log(responce);
      console.log(this.accountService.currentUser$);
    },error=>{
      console.log(error)
    });    
  }
  logout(){
    this.IsLoggedIn=false;
    this.accountService.logout();
  }
  // getCurrentUser(){
  //   this.accountService.currentUser$.subscribe(
  //     user=>{
  //       this.loggedIn=!!user;
  //     },
  //    error=>{
  //      console.log(console.error())
  //    })
  // }
}
