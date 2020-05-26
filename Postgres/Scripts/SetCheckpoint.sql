INSERT INTO __schema__.checkpoints (checkpoint_id, checkpoint)
VALUES (@CheckpointId, @Checkpoint)
ON CONFLICT (checkpoint_id)
DO UPDATE SET checkpoint = @Checkpoint