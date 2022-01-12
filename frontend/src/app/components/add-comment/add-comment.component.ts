import { Component, Input, OnInit } from '@angular/core';
import { error } from 'protractor';
import { Post } from 'src/app/models/post';
import { CommentService } from 'src/app/services/comment/comment.service';
import { Comment } from 'src/app/models/comment';
import { UserService } from 'src/app/services/user/user.service';

@Component({
  selector: 'app-add-comment',
  templateUrl: './add-comment.component.html',
  styleUrls: ['./add-comment.component.css']
})
export class AddCommentComponent implements OnInit {

  @Input() post: Post;
  @Input() comment: Comment = new Comment('','',0);
  value: string = '';

  constructor(private CommentService: CommentService, private UserService: UserService) { }

  ngOnInit(): void {
  }

  postComment() {
    this.comment.userRef = "dragan";//this.UserService.getUsername();
    this.comment.description = this.value;
    this.comment.isDeleted = 0;
    this.CommentService.postComment(this.comment, this.post.id.toString()).subscribe( () => this.post.comments.unshift(this.comment), error => console.log(error))
    // radi ali mora da napravim da se lepo dodaje komentar na stranicu, trenutno se ne prikaze osim ako rucno se ne reloda stranica
  }
}
