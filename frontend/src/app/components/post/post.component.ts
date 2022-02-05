import { Component, OnInit, Input, ChangeDetectorRef, SimpleChanges } from '@angular/core';
import { Comment } from 'src/app/models/comment';
import { Post } from 'src/app/models/post';
import { PostService } from 'src/app/services/post/post.service';
import { ActivatedRoute } from '@angular/router';
import { UserService } from 'src/app/services/user/user.service';
import { User } from 'src/app/models/user';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { error } from 'protractor';
import { CommentService } from 'src/app/services/comment/comment.service';

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
  private destroy$: Subject<void> = new Subject<void>();
  index: number = -1;
  update: boolean = false;
  textToUpdate: string;
  subscribedToAuthor: boolean;
  favoriteRecipe: boolean;

  constructor(private postService: PostService, private _Activatedroute:ActivatedRoute, private userService: UserService, private commentService: CommentService ) { }

  ngOnInit(): void {
    this.id=this._Activatedroute.snapshot.paramMap.get("id");
    this.postService.getRecipeById(this.id)
    .pipe(takeUntil(this.destroy$))
    .subscribe((resultPost) => {
      this.post = resultPost;
      this.comments = this.post.comments.filter(x=> x.isDeleted === 0);
      this.userService.getUser(localStorage.getItem("username"))
      .pipe(takeUntil(this.destroy$))
      .subscribe( resp2 => {
        this.currentUser = resp2
        if(this.currentUser.followedUsers.indexOf(this.post.userRef) !== -1)
          this.subscribedToAuthor = true
        else
          this.subscribedToAuthor = false
        if(this.currentUser.favoriteRecipes.filter( x => x.id === this.post.id).length !== 0)
          this.favoriteRecipe = true
        else
          this.favoriteRecipe = false
      })
    })
  }

  loggedIn(): boolean{
    return localStorage.getItem("username") === this.currentUser.username ? true : false;
  }

  ngOnChanges(){

  }

  commentAdded(comment: Comment): void{
    // const previous = this.post;
    // this.post = null;
    // previous.comments.unshift(comment);
    // this.post = previous;
    this.post.comments.unshift(comment)
    this.comments.unshift(comment)
  }

  commentDeleted(index: number): void {
    // const previous = this.post;
    // this.post = null;
    // previous.comments[index].isDeleted = 1;
    // this.post = previous;
    this.post.comments[index].isDeleted = 1;
    this.comments = this.post.comments.filter(x=> x.isDeleted === 0);
  }

  commentUpdated(resp: any): void{
    // const previous = this.post;
    // this.post = null;
    // previous.comments[resp.index] = resp.updatedComment
    // this.post = previous;
    this.post.comments[resp.index] = resp.updatedComment;
    this.comments = this.post.comments.filter(x=> x.isDeleted === 0);
  }

  isOwner(): boolean{
    return this.post.userRef === localStorage.getItem("username") ? true : false;
  }

  subscribeToAuthor(){
    this.userService.subscribeToAuthor(localStorage.getItem("username"), this.post.userRef)
    .pipe(takeUntil(this.destroy$))
    .subscribe( resp => {
      this.currentUser.followedUsers.push(this.post.userRef)
      this.subscribedToAuthor = true;
    })
  }

  unsubscribeFromAuthor(){
   this.userService.unsubscribeFromAuthor(localStorage.getItem("username"), this.post.userRef)
    .pipe(takeUntil(this.destroy$))
    .subscribe( resp => {
      this.currentUser.followedUsers.splice(this.currentUser.followedUsers.indexOf(this.post.userRef), 1);
      this.subscribedToAuthor = false;
    }, error => console.log(error))
  }

  addToFavorites(): void{
    this.userService.addToFavorites(localStorage.getItem("username"), this.post.id.toString())
    .pipe(takeUntil(this.destroy$))
    .subscribe( resp => {
      this.currentUser.favoriteRecipes.push(this.post)
      this.favoriteRecipe = true;
    })
  }

  removeFromFavorites(): void{
    this.userService.removeFromFavorites(localStorage.getItem("username"), this.post.id.toString())
    .pipe(takeUntil(this.destroy$))
    .subscribe( resp => {
      this.currentUser.favoriteRecipes = this.currentUser.favoriteRecipes.filter( x=> x.id !== this.post.id)
      this.favoriteRecipe = false;
    })
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
