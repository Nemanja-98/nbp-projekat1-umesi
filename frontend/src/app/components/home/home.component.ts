import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserService } from 'src/app/services/user/user.service';
import { PostService } from 'src/app/services/post/post.service';
import { Post } from 'src/app/models/post';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  public query :string = '';
  public highlightedPost = {
    title: '',
    id: -1,
    author: '',
  }
  public searchResult :Post[] = [];

  constructor(private http: HttpClient, public userService : UserService, public postService :PostService, public router : Router) { }

  ngOnInit(): void {
    this.postService.getAllRecipes().subscribe( (posts :Post[]) =>{ 

      this.searchResult = posts;

      const randomIndex :number = Math.round((Math.random() * 10)) % 5 ;
      console.log(randomIndex, posts, posts[randomIndex]);
      if(posts[randomIndex] != undefined){
        this.highlightedPost.title =  posts[randomIndex].title;
        this.highlightedPost.id = posts[randomIndex].id; 
        this.highlightedPost.author = posts[randomIndex].userRef;
      }
    });
  }

  highlightedPostClicked(){
    this.router.navigate([`/post/${this.highlightedPost.id}`]);
  }

  searchResultClicked(event){
    const id = (event.target.innerHTML as string).split('.')[0];
    console.log("searchresult clicked",id[1]);
    this.router.navigate([`post/${id[1]}`]);
  }
}
