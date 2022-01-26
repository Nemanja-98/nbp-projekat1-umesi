import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ContentMode } from 'src/app/models/contentMode';
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

  public username = "neki username"
  public displayedList :Array<string> = [];
  public displayedContentMode: ContentMode = ContentMode.Post;
  public showCreateNewRecipe : boolean = false;

  public recipeTitle :string = '';
  public recipeDesc :string = '';
  public recipeIngredients :string = '';
  
  constructor(
    private userService : UserService,
     private postService :PostService,
     private router :Router) {
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
    this.displayedContentMode = ContentMode.FavouritePost;
    
    this.userService.getUser(this.username);
    this.userService.user.subscribe( (user :User) => {
      this.displayedList = user.favoriteRecipes.map(( post :Post) => {
        return `${post.id}.${post.title}`;
      }); 
    })
    console.log("content Mode is:", this.displayedContentMode.displayMode);
  }

  showFollowedUsers(){
    this.showCreateNewRecipe = false;
    this.displayedContentMode = ContentMode.FollowedUser;
    
    this.userService.getUser(this.username);
    this,this.userService.user.subscribe( (user :User) => {
      this.displayedList = user.followedUsers.map( (username :string, index :number) =>{
        return `${index+1}.${username}`;
      }); 
    })
    
    console.log("content Mode is:", this.displayedContentMode.displayMode);
  }
  
  showUserPosts(){
    this.showCreateNewRecipe = false;
    this.displayedContentMode = ContentMode.Post;
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

    
    console.log("content Mode is:", this.displayedContentMode.displayMode);
  }

  showCreateNewRecipeForm(){
    this.showCreateNewRecipe = true;
    this.displayedContentMode = ContentMode.CreatePost;
    
    console.log("content Mode is:", this.displayedContentMode.displayMode);
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

    
    console.log("content Mode is:", this.displayedContentMode.displayMode);
  }

  parseIngredients(contents :string) : string[]{
    const parsedValues : string[] = contents.split('\n'); 
    return parsedValues;
  }

  routeToContent(event){
    const routeParam = (event.target.innerHTML as string).split('.');
    console.log("number is:", routeParam[0], routeParam[1] + " is the typeof content:", this.displayedContentMode.displayMode);
    
    const id: string = routeParam[0];
    const path :string = this.displayedContentMode.displayMode;
    
    this.router.navigate([`/${path}/${id}`]);
  }

  getMode(){
    return this.displayedContentMode.displayMode;
  }
}
