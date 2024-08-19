import { Drawing } from "src/drawing/drawing.entity";
import { Entity, BaseEntity, PrimaryGeneratedColumn, Column, ManyToOne } from "typeorm";

@Entity()
export class Reaction extends BaseEntity {
    @PrimaryGeneratedColumn()
    id: number;

    @Column()
    ipAddress: string;

    @Column()
    reaction: string;

    @ManyToOne(() => Drawing, drawing => drawing.reactions)
    drawing: Drawing;
}