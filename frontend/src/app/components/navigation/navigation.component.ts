import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from 'src/app/services/user/user.service';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.css']
})
export class NavigationComponent implements OnInit {

  constructor(public userService : UserService, public router :Router) { }

  ngOnInit(): void {
  }

  logout(): void{
    this.userService.logout()
  }

  randomRecipeClicked(){
    const id : number = Math.round(Math.random()*10) % 5;
    this.router.navigate([`/profile`]);
    setTimeout(() => {
      this.router.navigateByUrl(`post/${id}`);
    }, 0);
  }
}
