import { Component, Input, OnDestroy, OnInit, Output , EventEmitter} from '@angular/core';
import { Post } from 'src/app/models/post';
import { CommentService } from 'src/app/services/comment/comment.service';
import { Comment } from 'src/app/models/comment';
import { UserService } from 'src/app/services/user/user.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-add-comment',
  templateUrl: './add-comment.component.html',
  styleUrls: ['./add-comment.component.css']
})
export class AddCommentComponent implements OnInit, OnDestroy {

  @Input() post: Post;
  @Input() comment: Comment = new Comment('','',0);
  @Output() commentAdded: EventEmitter<Comment> = new EventEmitter<Comment>();
  private destroy$: Subject<void> = new Subject<void>();
  value: string = '';

  constructor(private CommentService: CommentService, private UserService: UserService) { }

  ngOnInit(): void {
  }

  postComment() {

    if(this.value === '') {
      alert("Ne mozete postaviti prazan komentar!");
      return;
    }

    this.comment.userRef = "dragan";//this.UserService.getUsername();
    this.comment.description = this.value;
    this.comment.isDeleted = 0;
    this.CommentService.postComment(this.comment, this.post.id.toString())
    .pipe(takeUntil(this.destroy$))
    .subscribe( resp => {
      console.log(this.comment)
      this.commentAdded.emit(this.comment);
      this.value='';
    }, error => console.log(error))
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
