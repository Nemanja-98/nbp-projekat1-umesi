import { Component, OnInit, Input, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil, timeout } from 'rxjs/operators';
import { Comment } from 'src/app/models/comment';
import { Post } from 'src/app/models/post';
import { CommentService } from 'src/app/services/comment/comment.service';
import { MatDialog } from '@angular/material/dialog';
import { ExampleDialogComponent } from '../example-dialog/example-dialog.component';

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
  @ViewChild('textareaRef') textareaElementRef: ElementRef;

  constructor(private CommentService: CommentService, public dialog: MatDialog) { }

  ngOnInit(): void {
  }

  updateComment(): void {
    this.index = this.post.comments.indexOf(this.comment)
    this.CommentService.updateComment(this.post.id, this.index, this.value)
    .pipe(takeUntil(this.destroy$))
    .subscribe( resp => {
      this.commentUpdated.emit()
      this.update = false;
    }, error => console.log("Upalo ovde", error))
  }

  isOwner(): boolean{
    return this.comment.userRef === localStorage.getItem("username") ? true : false;
  }

  showFormForUpdate() : void{
    this.update = true;
    this.value = this.comment.description;
    setTimeout(() => {
      this.textareaElementRef.nativeElement.focus();
    }, 0)
  }

  openDialog(): void {
    let dialogRef = this.dialog.open(ExampleDialogComponent);
    
    dialogRef.afterClosed().subscribe( resp => {
      if(resp === "true")
        this.deleteComment()
    })
  }

  deleteComment(): void {
    this.index = this.post.comments.indexOf(this.comment)
    this.CommentService.deleteComment(this.post.id, this.index)
    .pipe(takeUntil(this.destroy$))
    .subscribe( resp => {
      this.commentDeleted.emit()
    }, error => console.log("Upalo ovde", error))
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
