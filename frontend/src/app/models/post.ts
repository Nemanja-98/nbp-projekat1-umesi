import { Comment } from './comment'

export class Post {

    constructor(
        public Id: number,
        public UserRef: string,
        public Title: string,
        public Description: string,
        public ImagePath: string,
        public Comments: Comment[]) { }
}
