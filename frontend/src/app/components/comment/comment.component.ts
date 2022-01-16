import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Comment } from 'src/app/models/comment';
import { Post } from 'src/app/models/post';
import { CommentService } from 'src/app/services/comment/comment.service';

@Component({
  selector: 'app-comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.css']
})
export class CommentComponent implements OnInit {
  
  @Input() comment: Comment;
  @Input() post: Post;
  private destroy$: Subject<void> = new Subject<void>();
  index: number;
  @Output() commentDeleted: EventEmitter<number> = new EventEmitter<number>();

  constructor(private CommentService: CommentService) { }

  ngOnInit(): void {
    
  }

  deleteComment() {
    this.index = this.post.comments.indexOf(this.comment)
    console.log("POST ID", this.post.id, "INDEX:", this.index)
    this.CommentService.deleteComment(this.post.id, this.index)
    .pipe(takeUntil(this.destroy$))
    .subscribe( resp => {
      console.log("uspelo je")
      this.commentDeleted.emit(this.index)
    }, error => console.log("Upalo ovde", error))
  }

  updateComment() {
    this.index = this.post.comments.indexOf(this.comment)
    //dovde sam stao
  }


  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
