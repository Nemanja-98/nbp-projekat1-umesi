import { Component, OnInit, Input, ChangeDetectorRef, SimpleChanges } from '@angular/core';
import { Comment } from 'src/app/models/comment';
import { Post } from 'src/app/models/post';
import { PostService } from 'src/app/services/post/post.service';
import { ActivatedRoute } from '@angular/router';
import { UserService } from 'src/app/services/user/user.service';
import { User } from 'src/app/models/user';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css']
})
export class PostComponent implements OnInit {

  post: Post = new Post(1, "Miladin", "Gulas", "Lako se sprema", ["ulje", "jaja", "krompir", "meso"] ,[new Comment("Kika", "Dobro", 0), new Comment("Micko", "Super supica", 0)])
  id: string = "";
  comments: Comment[] = this.post.comments;
  currentUser: User;

  constructor(private postService: PostService, private _Activatedroute:ActivatedRoute, private userService: UserService ) { }

  ngOnInit(): void {
    this.id=this._Activatedroute.snapshot.paramMap.get("id");
    this.postService.getRecipeById(this.id).subscribe((resultPost) => {
      this.post = resultPost
      this.post.comments = this.post.comments.filter(x => x.isDeleted === 0);
      this.comments = this.post.comments
      console.log(this.post);
    })
    this.userService.user.subscribe((user: User) => this.currentUser = user)
  }

  loggedIn(): boolean{
    return localStorage.getItem("username") === this.currentUser.username ? true : false;
  }

  commentAdded(comment: Comment){
    console.log(comment)
    const previous = this.post
    this.post = null;
    previous.comments.unshift(comment);
    this.post = previous
    this.comments.unshift(comment)
  }
}
