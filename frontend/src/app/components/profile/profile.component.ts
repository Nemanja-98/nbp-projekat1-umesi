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
  public showCreateNewRecipe : boolean = false;

  public recipeTitle :string = '';
  public recipeDesc :string = '';
  public recipeIngredients :string = '';
  
  constructor(private userService : UserService) {
    this.displayedList = this.userService.getUserPosts(this.username);
    
    this.userService.user.subscribe( (user :User) => {
      console.log("nije vise neki username");
      if(user)
        this.username = user.username;//localStorage.getItem('username');
    })
  }

  ngOnInit(): void {
  }

  showFavouritePosts(){
    this.showCreateNewRecipe = false;
    this.displayedList = this.userService.getUserFavouritePosts(this.username);
  }

  showFollowedUsers(){
    this.showCreateNewRecipe = false;
    this.displayedList = this.userService.getFollowedUsers(this.username);
  }
  
  showUserPosts(){
    this.showCreateNewRecipe = false;
    this.displayedList = this.userService.getUserPosts(this.username);
  }

  showCreateNewRecipeForm(){
    this.showCreateNewRecipe = true;
  }

  createNewPost(){
    //http request preko post servis-a
  }
}
