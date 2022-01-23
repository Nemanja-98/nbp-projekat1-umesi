import { Component, OnInit } from '@angular/core';
import { Post } from 'src/app/models/post';
import { User } from 'src/app/models/user';
import { PostService } from 'src/app/services/post/post.service';
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
  
  constructor(private userService : UserService, private postService :PostService) {
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
    const posts = this.postService.getRecipesByUser();
    this.postService.userPosts$.subscribe( (posts :Post[]) => {
      if(posts){
        const userPosts: Post[] = posts.filter((post: Post) => post.userRef === this.username); 
        console.log("userposts", userPosts);
        this.displayedList = userPosts.map( (post :Post) => {
         return  `${post.id}.${post.title}`;
        })
      }
    });
  }

  showCreateNewRecipeForm(){
    this.showCreateNewRecipe = true;
  }

  createNewPost(){
    //http request preko post servis-a
    const  ingredients :string[] = this.parseIngredients(this.recipeIngredients);
    const recipeInfo = {
      title: this.recipeTitle,
      description: this.recipeDesc,
      ingredients: ingredients

    }
    this.postService.createNewRecipe(recipeInfo).subscribe((data) => {
      console.log("Created Recipe Status:", data);
    });
  }

  parseIngredients(contents :string) : string[]{
    const parsedValues : string[] = contents.split('\n'); 
    return parsedValues;
  }
}
