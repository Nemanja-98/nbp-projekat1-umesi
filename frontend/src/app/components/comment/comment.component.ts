import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { error } from 'protractor';
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
  @Input() value: string;
  @Output() commentDeleted: EventEmitter<number> = new EventEmitter<number>();
  @Output() commentUpdated: EventEmitter<any> = new EventEmitter<any>();
  private destroy$: Subject<void> = new Subject<void>();
  index: number;
  update: boolean = false;


  constructor(private CommentService: CommentService) { }

  ngOnInit(): void {
    
  }

  deleteComment(): void {
    this.index = this.post.comments.indexOf(this.comment)
    this.CommentService.deleteComment(this.post.id, this.index)
    .pipe(takeUntil(this.destroy$))
    .subscribe( resp => {
      this.commentDeleted.emit(this.index)
    }, error => console.log("Upalo ovde", error))
  }

  showFormForUpdate() : void{
    this.update = true;
    this.value = this.comment.description;
  }

  updateComment(): void {
    this.index = this.post.comments.indexOf(this.comment)
    this.CommentService.updateComment(this.post.id, this.index, this.value)
    .pipe(takeUntil(this.destroy$))
    .subscribe( resp => {
      this.commentUpdated.emit({updatedComment: resp, index: this.index})
    }, error => console.log("Upalo ovde", error))
  }

  isOwner(): boolean{
    return this.comment.userRef === localStorage.getItem("username") ? true : false;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
