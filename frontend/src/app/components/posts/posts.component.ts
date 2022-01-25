import { Component, OnInit } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { subscribeOn, takeUntil } from 'rxjs/operators';
import { Post } from 'src/app/models/post';
import { PostService } from 'src/app/services/post/post.service';

@Component({
  selector: 'app-posts',
  templateUrl: './posts.component.html',
  styleUrls: ['./posts.component.css']
})
export class PostsComponent implements OnInit {

  private destroy$: Subject<void> = new Subject<void>();
  posts: Post[];

  constructor(private postService: PostService) { }

  ngOnInit(): void {
    this.postService.getAllRecipes()
    .pipe(takeUntil(this.destroy$))
    .subscribe( resp => {
      this.posts = resp
      console.log(this.posts)
    })
  }

  

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
