import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/user';
import { UserService } from 'src/app/services/user/user.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  
  
  //to do update later
  public username = "neki username"
  public displayedList :Array<string> = [];
  
  
  constructor(private userService : UserService) {
    this.displayedList = this.userService.getUserPosts(this.username);
    
    this.userService.user.subscribe( (user :User) => {
      if(user)
        this.username = user.username;
    })
  }

  ngOnInit(): void {
  }

  showFavouritePosts(){
    this.displayedList = this.userService.getUserFavouritePosts(this.username);
  }

  showUserComments(){
    this.displayedList = this.userService.getUserComments(this.username);
  }
  
  showUserPosts(){
    this.displayedList = this.userService.getUserPosts(this.username);
  }
}
