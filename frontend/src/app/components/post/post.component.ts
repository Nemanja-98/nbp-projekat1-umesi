import { Component, OnInit } from '@angular/core';
import { Comment } from 'src/app/models/comment';
import { Post } from 'src/app/models/post';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css']
})
export class PostComponent implements OnInit {

  post: Post = new Post(1, "Miladin", "Gulas", "Lako se sprema", "https://material.angular.io/assets/img/examples/shiba2.jpg", [new Comment("Kika", "Dobro"), new Comment("Micko", "Super supica")])

  constructor() { }

  ngOnInit(): void {
    console.log(this.post.Comments)
  }

}
