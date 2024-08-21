import { CustomRepository } from "src/database/typeorm-ex.decorator";
import { Repository } from "typeorm";
import { Drawing } from "./drawing.entity";
import { PageOptionsDto } from "src/config/page/page-option.dto";

@CustomRepository(Drawing)
export class DrawingRepository extends Repository<Drawing> {   
  

    // Find all drawings by a specific user
    async findByUserName(userName: string): Promise<Drawing[]> {
        return await this.find({ where: { userName } });
    }

    async createDrawing(drawingData: Partial<Drawing>): Promise<Drawing> {
        const drawing = this.create(drawingData);
        return await this.save(drawing);
    }

    async updateDrawing(id: number, updateData: Partial<Drawing>): Promise<Drawing> {
        await this.update(id, updateData);
        return await this.findOne({ where: { id } });
    }

    async findDrawingById(id: number): Promise<Drawing> {
        return await this.findOne({ where: { id } });
    }

    async deleteDrawing(id: number): Promise<void> {
        await this.delete(id);
    }

  

    async findRandomDrawings(pageOptionsDto: PageOptionsDto): Promise<[Drawing[], number]> {
        try {
            const queryBuilder = this.createQueryBuilder("drawing");

            queryBuilder
                .orderBy("RANDOM()")
                .skip(pageOptionsDto.skip)
                .take(pageOptionsDto.take);

            const [items, totalItems] = await queryBuilder.getManyAndCount();

            return [items, totalItems];
        } catch (error) {
            console.error("Error fetching random drawings:", error);
            throw new Error("Could not fetch random drawings");
        }
    }

    async findAllDrawings(pageOptionsDto: PageOptionsDto): Promise<[Drawing[], number]> {
        try {
            const queryBuilder = this.createQueryBuilder("drawing");

            queryBuilder
                .orderBy("drawing.createdDate", pageOptionsDto.order)
                .skip(pageOptionsDto.skip)
                .take(pageOptionsDto.take);

            const [items, totalItems] = await queryBuilder.getManyAndCount();

            return [items, totalItems];
        } catch (error) {
            console.error("Error fetching drawings with pagination:", error);
            throw new Error("Could not fetch drawings with pagination");
        }
    }

    async findDrawingsOrderedByLikes(pageOptionsDto: PageOptionsDto): Promise<[Drawing[], number]> {
        try {
            const queryBuilder = this.createQueryBuilder("drawing");

            queryBuilder
                .addSelect("array_length(drawing.like, 1)", "likeCount")
                .orderBy("likeCount", pageOptionsDto.order)
                .skip(pageOptionsDto.skip)
                .take(pageOptionsDto.take);

            const [items, totalItems] = await queryBuilder.getManyAndCount();

            return [items, totalItems];
        } catch (error) {
            console.error("Error fetching drawings ordered by likes:", error);
            throw new Error("Could not fetch drawings ordered by likes");
        }
    }

    
}