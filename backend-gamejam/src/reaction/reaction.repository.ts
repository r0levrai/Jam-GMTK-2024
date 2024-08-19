import { Repository } from 'typeorm';
import { Reaction } from './reaction.entity';
import { CustomRepository } from 'src/database/typeorm-ex.decorator';
import { Drawing } from 'src/drawing/drawing.entity';

@CustomRepository(Reaction)
export class ReactionRepository extends Repository<Reaction> {

    async findByDrawingAndIpAddress(drawing: Drawing, ipAddress: string) {
        return await this.findOne({ where: { drawing: drawing, ipAddress: ipAddress } });
    }
}